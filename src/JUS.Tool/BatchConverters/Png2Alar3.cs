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
using System.Linq;
using JUSToolkit.Containers;
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
    /// Inserts a PNG into an Alar3.
    /// </summary>
    public class Png2Alar3 :
        IConverter<Alar3, Alar3>
    {
        private NodeContainerFormat transformedFiles; // Dig + Atm to insert in the Alar3

        /// <summary>
        /// Initializes a new instance of the <see cref="Png2Alar3"/> class.
        /// </summary>
        /// <param name="image">PNG to insert.</param>
        /// <param name="digName">Name of the Dig.</param>
        /// <param name="atmName">Name of the atm.</param>
        public Png2Alar3(Node image, string digName, string atmName)
        {
            Image = image;
            DigName = digName;
            AtmName = atmName;
        }

        /// <summary>
        /// Gets or sets the PNG we are inserting.
        /// </summary>
        public Node Image { get; set; }

        /// <summary>
        /// Gets or sets the original name of the Dig of the image.
        /// </summary>
        public string DigName { get; set; }

        /// <summary>
        /// Gets or sets the original name of the Atm of the image.
        /// </summary>
        public string AtmName { get; set; }

        /// <summary>
        /// Converts a <see cref="Node"/> (png file) to a <see cref="Alar3"/> container.
        /// </summary>
        /// <param name="originalAlar">Original Alar3.</param>
        /// <returns><see cref="Alar3"/>Alar3 with the PNG inserted.</returns>
        public Alar3 Convert(Alar3 originalAlar)
        {
            if (Path.GetExtension(Image.Name) != ".png") {
                throw new FormatException("Invalid png file");
            }

            transformedFiles = new NodeContainerFormat();

            // Obtaining the original Dig and Almt
            Node dig = Navigator.IterateNodes(originalAlar.Root).First(n => n.Name == DigName) ?? throw new FormatException("Dig doesn't exist: " + DigName);
            Node atm = Navigator.IterateNodes(originalAlar.Root).First(n => n.Name == AtmName) ?? throw new FormatException("Atm doesn't exist: " + AtmName);

            // Clone the nodes
            var dig_clone = (BinaryFormat)new BinaryFormat(dig.Stream).DeepClone();
            var atm_clone = (BinaryFormat)new BinaryFormat(atm.Stream).DeepClone();

            // Transform the PNG into the new Dig and Almt (we need the original dig + atm)
            Transform(Image, new Node(dig.Name, dig_clone), new Node(atm.Name, atm_clone));

            originalAlar.InsertModification(transformedFiles);

            return originalAlar;
        }

        private void Transform(Node png, Node dig, Node atm)
        {
            bool digIsCompressed = CompressionUtils.IsCompressed(dig);
            Node uncompressedDig = digIsCompressed ?
                dig.TransformWith<LzssDecompression>() :
                dig;

            Dig originalDig = uncompressedDig
                .TransformWith<Binary2Dig>()
                .GetFormatAs<Dig>() ?? throw new FormatException("Invalid dig file");

            bool atmIsCompressed = CompressionUtils.IsCompressed(atm);
            Node uncompressedAtm = atmIsCompressed ?
                atm.TransformWith<LzssDecompression>() :
                atm;

            Almt originalAtm = uncompressedAtm
                    .TransformWith<Binary2Almt>()
                    .GetFormatAs<Almt>() ?? throw new FormatException("Invalid atm file");

            var compressionParams = new FullImageMapCompressionParams {
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
    }
}
