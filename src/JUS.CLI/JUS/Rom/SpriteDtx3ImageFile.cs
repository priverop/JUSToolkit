// Copyright (c) 2026 Priverop

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System.Text.RegularExpressions;
using JUS.Tool.Containers;
using JUS.Tool.Containers.Converters;
using JUS.Tool.Graphics.Converters;
using JUS.Tool.Utils;
using YamlDotNet.RepresentationModel;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.CLI.JUS.Rom
{
    /// <summary>
    /// Strategy to import DTX3 sprites. These sprites are usually inside a parent .aar and child .aar.
    /// Filename format: parent.aar-child.aar-name.dtx-sp_NN.png, or parent.aar-name.dtx-sp_NN.png
    /// when the .dtx hangs directly from the parent .aar (jquiz question images or Commu/ .dtx).
    /// </summary>
    public class SpriteDtx3ImageFile : IFileImportStrategy
    {
        private static readonly Regex FilenamePattern = new(@"^[^-]+\.aar-([^-]+\.aar-)?[^-]+\.dtx-sp_\d+\.png$", RegexOptions.Compiled);

        // The importer assumes the /data/parent directory name is the same as the parent.aar file.
        // This is not the case for some of the .aar
        private static readonly Dictionary<string, string> ParentLocations = new() {
            { "button.aar", "Common" },
            { "commu_pack.aar", "Commu" },
            { "error_2d.aar", "Commu" },
            { "jquiz_pack.aar", "jquiz" },
            { "pause.aar", "battle" },
            { "title_icon_2d.aar", "Common" },
        };

        // Some .dtx hang directly from the parent.aar, without an intermediate child.aar.
        // They are at its root unless listed here, where the value is their inner directory.
        private static readonly Dictionary<string, string> DirectDtxSubdirectories = new() {
            { "jquiz_pack.aar", "jquiz/img" },
        };

        /// <inheritdoc/>
        public bool Matches(string filename)
        {
            return FilenamePattern.IsMatch(filename);
        }

        /// <inheritdoc/>
        public void Import(Node gameNode, List<Node> files)
        {
            // filename: [0] parent.aar, [1] child.aar, [2] name.dtx, [3] sp_NN.png
            var filesGroupedByParent = files.GroupBy(f => f.Name.Split('-')[0]);

            foreach (var parentGroup in filesGroupedByParent) {
                string parentName = parentGroup.Key;
                string parentPath = Path.GetFileNameWithoutExtension(parentName);
                ProcessParentContainer(gameNode, parentPath, parentGroup);
            }
        }

        // Process parent.aar (Alar3)
        private static void ProcessParentContainer(Node gameNode, string parentPath, IEnumerable<Node> files)
        {
            string parentDirectory = ParentLocations.TryGetValue($"{parentPath}.aar", out string? directory) ? directory : parentPath;

            Node parentAlar = Navigator.SearchNode(gameNode, $"/root/data/{parentDirectory}/{parentPath}.aar") ?? throw new FormatException($"Container not found /root/data/{parentDirectory}/{parentPath}.aar");

            Console.WriteLine($"/root/data/{parentDirectory}/{parentPath}.aar found.");

            bool isCompressed = CompressionUtils.IsCompressed(parentAlar);

            _ = parentAlar.TransformWith<Binary2Alar>();
            parentAlar.Tags[Alar.CompressionTag] = isCompressed;

            // A parent.aar can hold both layouts, so we cannot decide by container.
            string dtxRoot = DirectDtxSubdirectories.TryGetValue($"{parentPath}.aar", out string? subdirectory) ? $"{subdirectory}/" : string.Empty;

            // filename: [0] parent.aar, [1] name.dtx, [2] sp_NN.png
            var filesGroupedByDtx = files.Where(f => f.Name.Split('-').Length == 3).GroupBy(f => f.Name.Split('-')[1]);

            foreach (var dtxGroup in filesGroupedByDtx) {
                ProcessDtx(parentAlar, $"{dtxRoot}{dtxGroup.Key}", dtxGroup);
            }

            var filesGroupedByChild = files.Where(f => f.Name.Split('-').Length == 4).GroupBy(f => f.Name.Split('-')[1]);

            foreach (var childGroup in filesGroupedByChild) {
                ProcessChildContainer(parentAlar, parentDirectory, childGroup.Key, childGroup);
            }

            _ = parentAlar.TransformWith(new AlarToBinary());
        }

        // Process child.aar (Alar2, usually compressed).
        // The directory holding it inside the parent.aar is named after the /data one, not after the parent.aar.
        private static void ProcessChildContainer(Node parentAlar, string childDirectory, string childName, IEnumerable<Node> files)
        {
            Node originalChild = Navigator.SearchNode(parentAlar, $"{parentAlar.Path}/{childDirectory}/{childName}")
                ?? throw new FormatException($"Container not found {parentAlar.Path}/{childDirectory}/{childName}");

            Console.WriteLine($"{childName} found.");

            // Clone the node to avoid changing the original (AlarFile).
            using var workingChild = new Node(childName, (BinaryFormat)new BinaryFormat(originalChild.Stream!).DeepClone());
            bool isCompressed = CompressionUtils.IsCompressed(workingChild);

            _ = workingChild.TransformWith<Binary2Alar>();
            workingChild.Tags[Alar.CompressionTag] = isCompressed;

            var filesGroupedByDtx = files.GroupBy(f => f.Name.Split('-')[2]);

            foreach (var dtxGroup in filesGroupedByDtx) {
                ProcessDtx(workingChild, dtxGroup.Key, dtxGroup);
            }

            BinaryFormat childBinary = new AlarToBinary().Convert(workingChild.GetFormatAs<Alar>()!);

            using var newChild = new Node(childName, childBinary);
            parentAlar.GetFormatAs<Alar>()!.InsertModification(newChild);
        }

        private static void ProcessDtx(Node childAlar, string dtxPath, IEnumerable<Node> files)
        {
            Node originalDTX = Navigator.SearchNode(childAlar, dtxPath) ?? throw new FormatException($"DTX {dtxPath} not found in {childAlar.Path}.");

            string dtxName = Path.GetFileName(dtxPath);

            Console.WriteLine($"Importing sprites into: {dtxName}.");

            // Clone the node to avoid changing the original (AlarFile)
            using var workingDtx = new Node(dtxName, (BinaryFormat)new BinaryFormat(originalDTX.Stream!).DeepClone());
            bool isCompressed = CompressionUtils.IsCompressed(workingDtx);

            if (isCompressed) {
                _ = workingDtx.TransformWith<LzssDecompression>();
            }

            _ = workingDtx.TransformWith<BinaryToDtx3>();

            // Renamed and cloned (just in case) TODO: quitar?
            using var pngs = new NodeContainerFormat();
            foreach (Node file in files) {
                pngs.Root.Add(new Node(file.Name.Split('-')[^1], new BinaryFormat(file.Stream!)));
            }

            _ = workingDtx.TransformWith(new Png2Dtx3(pngs));

            BinaryFormat dtxBinary = new Dtx3ToBinary().Convert(workingDtx.GetFormatAs<NodeContainerFormat>()!);

            BinaryFormat compressedDtx = isCompressed ?
                new LzssCompression().Convert(dtxBinary) :
                dtxBinary;

            using var newDtx = new Node(dtxName, compressedDtx);
            containerAlar.GetFormatAs<Alar>()!.InsertModification(newDtx);
        }
    }
}
