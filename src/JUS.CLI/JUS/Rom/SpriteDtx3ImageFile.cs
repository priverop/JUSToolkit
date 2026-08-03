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
    /// Filename format: parent.aar-child.aar-name.dtx-sp_NN.png.
    /// </summary>
    public class SpriteDtx3ImageFile : IFileImportStrategy
    {
        private static readonly Regex FilenamePattern = new(@"^[^-]+\.aar-[^-]+\.aar-[^-]+\.dtx-sp_\d+\.png$", RegexOptions.Compiled);

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

        private static void ProcessParentContainer(Node gameNode, string parentPath, IEnumerable<Node> files)
        {
            Node parentAlar = Navigator.SearchNode(gameNode, $"/root/data/{parentPath}/{parentPath}.aar") ?? throw new FormatException($"Container not found /root/data/{parentPath}/{parentPath}.aar");

            Console.WriteLine($"/root/data/{parentPath}/{parentPath}.aar found.");

            _ = parentAlar.TransformWith<Binary2Alar>();

            var filesGroupedByChild = files.GroupBy(f => f.Name.Split('-')[1]);

            foreach (var childGroup in filesGroupedByChild) {
                ProcessChildContainer(parentAlar, childGroup.Key, childGroup);
            }

            _ = parentAlar.TransformWith(new AlarToBinary());
        }

        private static void ProcessChildContainer(Node parentAlar, string childName, IEnumerable<Node> files)
        {
            Node originalChild = Navigator.SearchNode(parentAlar, $"{parentAlar.Path}/{Path.GetFileNameWithoutExtension(parentAlar.Name)}/{childName}")
                ?? throw new FormatException($"Container not found {parentAlar.Path}/{childName}");

            Console.WriteLine($"{childName} found.");

            // Clone the node to avoid changing the original (AlarFile).
            using var workingChild = new Node(childName, (BinaryFormat)new BinaryFormat(originalChild.Stream!).DeepClone());
            _ = workingChild.TransformWith<Binary2Alar>();

            var filesGroupedByDtx = files.GroupBy(f => f.Name.Split('-')[2]);

            foreach (var dtxGroup in filesGroupedByDtx) {
                ProcessDtx(workingChild, dtxGroup.Key, dtxGroup);
            }

            BinaryFormat childBinary = new AlarToBinary().Convert(workingChild.GetFormatAs<Alar>()!);

            using var newChild = new Node(childName, childBinary);
            parentAlar.GetFormatAs<Alar>()!.InsertModification(newChild);
        }

        private static void ProcessDtx(Node childAlar, string dtxName, IEnumerable<Node> files)
        {
            Node originalDTX = Navigator.SearchNode(childAlar, dtxName) ?? throw new FormatException($"DTX {dtxName} not found in {childAlar.Path}.");

            Console.WriteLine($"Importing sprites into: {dtxName}.");

            // Clone the node to avoid changing the original (AlarFile)
            using var workingDtx = new Node(dtxName, (BinaryFormat)new BinaryFormat(originalDTX.Stream!).DeepClone());
            _ = workingDtx.TransformWith<BinaryToDtx3>();

            // Renamed and cloned (just in case) TODO: quitar?
            using var pngs = new NodeContainerFormat();
            foreach (Node file in files) {
                pngs.Root.Add(new Node(file.Name.Split('-')[3], new BinaryFormat(file.Stream!)));
            }

            _ = workingDtx.TransformWith(new Png2Dtx3(pngs));

            BinaryFormat dtxBinary = new Dtx3ToBinary().Convert(workingDtx.GetFormatAs<NodeContainerFormat>()!);

            using var newDtx = new Node(dtxName, dtxBinary);
            childAlar.GetFormatAs<Alar>()!.InsertModification(newDtx);
        }
    }
}
