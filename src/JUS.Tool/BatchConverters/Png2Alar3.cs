// Copyright (c) 2022 Priverop

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
using System;
using System.IO;
using JUSToolkit.Containers;
using JUSToolkit.Containers.Converters;
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using JUSToolkit.Utils;
using Texim.Compressions.Nitro;
using Texim.Formats;
using Texim.Images;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.BatchConverters
{
    /// <summary>
    /// Converts a bunch of PNGs to and Alar3.
    /// </summary>
    public class Png2Alar3 :
        IConverter<NodeContainerFormat, Alar3>
    {
        private NodeContainerFormat transformedFiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Png2Alar3"/> class.
        /// </summary>
        /// <param name="alar">Original Alar.</param>
        public Png2Alar3(Node alar)
        {
            OriginalAlar = alar;
        }

        /// <summary>
        /// Gets or sets the Original Alar.
        /// </summary>
        public Node OriginalAlar { get; set; }

        /// <summary>
        /// Converts a <see cref="BinaryFormat"/> (files) to a <see cref="Alar3"/> container.
        /// </summary>
        /// <param name="source">Files to convert.</param>
        /// <returns><see cref="Alar3"/>.</returns>
        public Alar3 Convert(NodeContainerFormat source)
        {
            Alar3 alar = OriginalAlar
                .TransformWith<Binary2Alar3>()
                .GetFormatAs<Alar3>() ?? throw new FormatException("Invalid container file");

            NodeContainerFormat filesToInsert = source;
            transformedFiles = new NodeContainerFormat();

            foreach (Node file in source.Root.Children)
            {
                if (Path.GetExtension(file.Name) == ".png")
                {
                    string cleanName = Path.GetFileNameWithoutExtension(file.Name);
                    NodeContainerFormat originals = GetOriginals(cleanName, filesToInsert.Root);
                    Transform(new Node(file), originals.Root.Children[cleanName + ".dig"], originals.Root.Children[cleanName + ".atm"]);
                }
            }

            alar.InsertModification(transformedFiles.Root);

            return alar;
        }

        private void Transform(Node png, Node dig, Node atm)
        {
            bool digIsCompressed = CompressionUtils.IsCompressed(dig);
            Node uncompressedDig = digIsCompressed ?
                dig.TransformWith<LzssDecompression>() :
                dig;

            Dig originalDig = uncompressedDig
                .TransformWith<Binary2Dig>()
                .GetFormatAs<Dig>();

            bool atmIsCompressed = CompressionUtils.IsCompressed(atm);
            Node uncompressedAtm = atmIsCompressed ?
                atm.TransformWith<LzssDecompression>() :
                atm;

            Almt originalAtm = uncompressedAtm
                    .TransformWith<Binary2Almt>()
                    .GetFormatAs<Almt>();

            if (originalDig is null)
            {
                throw new FormatException("Invalid dig file");
            }

            if (originalAtm is null)
            {
                throw new FormatException("Invalid atm file");
            }

            var compressionParams = new FullImageMapCompressionParams
            {
                Palettes = originalDig,
            };

            png.Stream.Position = 0;
            var mapCompression = new FullImageMapCompression(compressionParams);
            Node compressed = png
                .TransformWith<Bitmap2FullImage>()
                .TransformWith(mapCompression);
            IndexedImage newImage = compressed.Children[0].GetFormatAs<IndexedImage>();
            ScreenMap map = compressed.Children[1].GetFormatAs<ScreenMap>();

            // Dig
            var newDig = new Dig(originalDig, newImage);
            BinaryFormat binaryDig = new Dig2Binary().Convert(newDig);

            BinaryFormat compressedDig = digIsCompressed ?
                new LzssCompression().Convert(binaryDig) :
                binaryDig;

            transformedFiles.Root.Add(new Node(dig.Name, compressedDig));

            // Atm
            var newAtm = new Almt(originalAtm, map);
            BinaryFormat binaryAtm = new Almt2Binary().Convert(newAtm);

            BinaryFormat compressedAtm = atmIsCompressed ?
                new LzssCompression().Convert(binaryAtm) :
                binaryAtm;

            transformedFiles.Root.Add(new Node(atm.Name, compressedAtm));
        }

        private NodeContainerFormat GetOriginals(string name, Node files)
        {
            var originals = new NodeContainerFormat();

            Node dig = Navigator.SearchNode(files, name + ".dig") ?? throw new FormatException("Dig doesn't exist: " + name + ".dig");

            Node atm = Navigator.SearchNode(files, name + ".atm") ?? throw new FormatException("Atm doesn't exist: " + name + ".atm");

            originals.Root.Add(new Node(dig));
            originals.Root.Add(new Node(atm));

            return originals;
        }
    }
}
