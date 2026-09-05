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
using JUS.Tool.Graphics;
using JUS.Tool.Graphics.Converters;
using JUS.Tool.Utils;
using Texim.Formats.ImageSharp.Images;
using Texim.Images;
using Texim.Images.Quantization;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.CLI.JUS.Rom
{
    /// <summary>
    /// Strategy to import the base image of DTX3TX sprites. These sprites are usually inside a
    /// parent .aar and child .aar.
    /// Filename format: parent[-child.aar]-name.dtx-tx.png,
    /// Optional parent[-child.aar]-name.dtx-tx.yaml next to the PNG rewrites the segments.
    /// </summary>
    public class SpriteDtx3TxImageFile : IFileImportStrategy
    {
        private static readonly Regex FilenamePattern = new(@"^[^-]+-([^-]+\.aar-)?[^-]+\.dtx-tx\.(png|yaml)$", RegexOptions.Compiled);

        // The importer assumes the /data/parent directory name is the same as the parent.aar file.
        // This is not the case for some of the .aar.
        private static readonly Dictionary<string, string> ParentLocations = new() {
            { "button.aar", "Common" },
            { "challenge_3d.aar", "Commu" },
            { "commu_pack.aar", "Commu" },
            { "error_2d.aar", "Commu" },
            { "get.aar", "battle" },
            { "jquiz_pack.aar", "jquiz" },
            { "ko.aar", "battle" },
            { "marker_b.aar", "battle" },
            { "marker_t.aar", "battle" },
            { "pause.aar", "battle" },
            { "set.aar", "battle" },
            { "title_icon_2d.aar", "Common" },
            { "tutorial_a.aar", "battle" },
        };

        /// <inheritdoc/>
        public bool Matches(string filename)
        {
            return FilenamePattern.IsMatch(filename);
        }

        /// <inheritdoc/>
        public void Import(Node gameNode, List<Node> files)
        {
            foreach (var parentGroup in files.GroupBy(ParentOf)) {
                ProcessParentContainer(gameNode, parentGroup.Key, parentGroup);
            }
        }

        // Process parent.aar (Alar3)
        private static void ProcessParentContainer(Node gameNode, string parentName, IEnumerable<Node> files)
        {
            // By default the directory is named after the .aar (deckselect.aar -> /data/deckselect).
            string parentDirectory = ParentLocations.TryGetValue(parentName, out string? directory)
                ? directory
                : Path.GetFileNameWithoutExtension(parentName);

            Node parentAlar = Navigator.GetNode(gameNode, $"/root/data/{parentDirectory}/{parentName}");

            Console.WriteLine($"/root/data/{parentDirectory}/{parentName} found.");

            bool isCompressed = CompressionUtils.IsCompressed(parentAlar);

            _ = parentAlar.TransformWith<Binary2Alar>();
            parentAlar.Tags[Alar.CompressionTag] = isCompressed;

            // No child.aar
            foreach (var dtxGroup in files.Where(f => ChildOf(f) is null).GroupBy(DtxOf)) {
                ProcessDtx(parentAlar, dtxGroup.Key, dtxGroup);
            }

            foreach (var childGroup in files.Where(f => ChildOf(f) is not null).GroupBy(f => ChildOf(f)!)) {
                ProcessChildContainer(parentAlar, childGroup.Key, childGroup);
            }

            _ = parentAlar.TransformWith(new AlarToBinary());
        }

        // Process child.aar (Alar2, usually compressed)
        private static void ProcessChildContainer(Node parentAlar, string childName, IEnumerable<Node> files)
        {
            Node originalChild = FindByName(parentAlar, childName);

            Console.WriteLine($"{childName} found.");

            // Clone the node to avoid changing the original (AlarFile).
            using var workingChild = new Node(childName, (BinaryFormat)new BinaryFormat(originalChild.Stream).DeepClone());
            bool isCompressed = CompressionUtils.IsCompressed(workingChild);

            _ = workingChild.TransformWith<Binary2Alar>();
            workingChild.Tags[Alar.CompressionTag] = isCompressed;

            foreach (var dtxGroup in files.GroupBy(DtxOf)) {
                ProcessDtx(workingChild, dtxGroup.Key, dtxGroup);
            }

            BinaryFormat childBinary = new AlarToBinary().Convert(workingChild.GetFormatAs<Alar>());

            using var newChild = new Node(childName, childBinary);
            parentAlar.GetFormatAs<Alar>().InsertModification(newChild);
        }

        private static void ProcessDtx(Node containerAlar, string dtxName, IEnumerable<Node> files)
        {
            Node originalDtx = FindByName(containerAlar, dtxName);

            Console.WriteLine($"Importing texture into: {dtxName}.");

            // Clone the node to avoid changing the original (AlarFile)
            using var workingDtx = new Node(dtxName, (BinaryFormat)new BinaryFormat(originalDtx.Stream).DeepClone());
            bool isCompressed = CompressionUtils.IsCompressed(workingDtx);

            if (isCompressed) {
                _ = workingDtx.TransformWith<LzssDecompression>();
            }

            // We need the original for the Dtx3TxToBinary converter
            var decompressedDtx = (BinaryFormat)new BinaryFormat(workingDtx.Stream).DeepClone();

            _ = workingDtx.TransformWith<BinaryToDtx3>();

            Dig originalImage = workingDtx.Children["image"].GetFormatAs<Dig>();

            if (originalImage.Swizzling != DigSwizzling.Linear) {
                throw new FormatException($"{dtxName} is not a Dtx3Tx.");
            }

            var quantization = new FixedPaletteQuantization(originalImage.Palettes[0]);

            using var pngNode = new Node(dtxName, new BinaryFormat(PngOf(files, dtxName).Stream));
            _ = pngNode.TransformWith(new StandardBinaryImage2IndexedPaletteImage(quantization));

            IndexedPaletteImage newImage = pngNode.GetFormatAs<IndexedPaletteImage>();

            var updatedImage = new Dig(originalImage) {
                Pixels = newImage.Pixels.ToArray(),
            };

            workingDtx.Children["image"].ChangeFormat(updatedImage);

            // Optional YAML
            Node? yaml = files.FirstOrDefault(f => Path.GetExtension(f.Name) == ".yaml");

            Dtx3TxToBinary converter;

            if (yaml is null) {
                converter = new Dtx3TxToBinary(decompressedDtx);
            } else {
                var reader = new TextDataReader(yaml.Stream);
                reader.Stream.Position = 0;

                converter = new Dtx3TxToBinary(decompressedDtx, BinaryToDtx3.DeserializeYaml(reader.ReadToEnd()));
            }

            BinaryFormat dtxBinary = converter.Convert(workingDtx.GetFormatAs<NodeContainerFormat>());

            BinaryFormat compressedDtx = isCompressed ?
                new LzssCompression().Convert(dtxBinary) :
                dtxBinary;

            using var newDtx = new Node(dtxName, compressedDtx);
            containerAlar.GetFormatAs<Alar>().InsertModification(newDtx);
        }

        // filename: parent[-child.aar]-name.dtx-tx.png|yaml
        private static string ParentOf(Node file) => file.Name.Split('-')[0] + ".aar";

        // Null when the .dtx has no child.aar.
        private static string? ChildOf(Node file)
        {
            string[] segments = file.Name.Split('-');
            return segments.Length == 4 ? segments[1] : null;
        }

        private static string DtxOf(Node file) => file.Name.Split('-')[^2];

        private static Node PngOf(IEnumerable<Node> files, string dtxName)
        {
            return files.FirstOrDefault(f => Path.GetExtension(f.Name) == ".png")
                ?? throw new FormatException($"Missing the .png image of {dtxName}.");
        }

        private static Node FindByName(Node container, string name)
        {
            return Navigator.IterateNodes(container).FirstOrDefault(n => n.Name == name)
                ?? throw new FormatException($"{name} not found in {container.Path}.");
        }
    }
}
