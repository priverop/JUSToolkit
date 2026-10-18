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
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.CLI.JUS.Rom
{
    /// <summary>
    /// Strategy to import komas (DTX4).
    /// Filename format: komas-manga_NN.png (e.g. komas-bb_01.png for bb_01.dtx).
    /// </summary>
    public class KomaDtx4ImageFile : IFileImportStrategy
    {
        private static readonly Regex FilenamePattern = new(@"^komas-[^-]+\.png$", RegexOptions.Compiled);

        /// <inheritdoc/>
        public bool Matches(string filename)
        {
            return FilenamePattern.IsMatch(filename);
        }

        /// <inheritdoc/>
        public void Import(Node gameNode, List<Node> files)
        {
            Node komaAlar = Navigator.GetNode(gameNode, "/root/data/koma/koma.aar") ?? throw new FormatException("Container not found /root/data/koma/koma.aar");
            Node komaBin = Navigator.GetNode(gameNode, "/root/data/bin/koma.bin") ?? throw new FormatException("File not found /root/data/bin/koma.bin");
            Node kshapeBin = Navigator.GetNode(gameNode, "/root/data/bin/kshape.bin") ?? throw new FormatException("File not found /root/data/bin/kshape.bin");

            Console.WriteLine("Koma.aar, koma.bin, and kshape.bin found.");

            Koma koma = new Binary2Koma().Convert(komaBin.GetFormatAs<IBinary>());
            KShapeSprites shapes = new BinaryKShape2SpriteCollection().Convert(kshapeBin.GetFormatAs<IBinary>());

            bool isCompressed = CompressionUtils.IsCompressed(komaAlar);

            _ = komaAlar.TransformWith<Binary2Alar>();
            komaAlar.Tags[Alar.CompressionTag] = isCompressed;

            foreach (Node file in files) {
                ProcessKoma(komaAlar, file, koma, shapes);
            }

            _ = komaAlar.TransformWith(new AlarToBinary());
        }

        private static void ProcessKoma(Node komaAlar, Node file, Koma koma, KShapeSprites shapes)
        {
            // filename: komas-manga_NN.png
            string komaName = Path.GetFileNameWithoutExtension(file.Name)[6..]; // "komas-" 6 characters
            string dtxName = $"{komaName}.dtx";

            KomaElement komaElement = koma.FirstOrDefault(k => k.KomaName == komaName)
                ?? throw new FormatException($"Can't find '{komaName}' in the koma.bin");

            Node originalDtx = Navigator.IterateNodes(komaAlar).FirstOrDefault(n => n.Name == dtxName)
                ?? throw new FormatException($"{dtxName} not found in {komaAlar.Path}.");

            Console.WriteLine($"Importing koma into: {dtxName}.");

            // Clone the node to avoid changing the original (AlarFile)
            using var workingDtx = new Node(dtxName, (BinaryFormat)new BinaryFormat(originalDtx.Stream).DeepClone());
            bool isCompressed = CompressionUtils.IsCompressed(workingDtx);

            if (isCompressed) {
                _ = workingDtx.TransformWith<LzssDecompression>();
            }

            _ = workingDtx.TransformWith<BinaryDtx4ToSpriteImage>() // NCF with sprite+image
                .TransformWith(new Png2Dtx4(file.GetFormatAs<IBinary>(), shapes, komaElement));

            BinaryFormat dtxBinary = new Dtx4ToBinary().Convert(workingDtx.GetFormatAs<NodeContainerFormat>());

            BinaryFormat compressedDtx = isCompressed ?
                new LzssCompression().Convert(dtxBinary) :
                dtxBinary;

            using var newDtx = new Node(dtxName, compressedDtx);
            komaAlar.GetFormatAs<Alar>().InsertModification(newDtx);
        }
    }
}
