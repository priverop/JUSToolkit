// Copyright (c) 2024 Priverop

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
using JUS.Tool.Containers;
using JUS.Tool.Graphics;
using JUS.Tool.Graphics.Converters;
using JUS.Tool.Utils;
using Texim.Images;
using Texim.Images.Standard;
using Texim.TileMaps;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.BatchConverters
{
    /// <summary>
    /// Inserts a PNG into an Alar3.
    /// </summary>
    public class Demo2Alar3 :
        IConverter<Alar, Alar>
    {
        private NodeContainerFormat transformedFiles = null!; // Dig + Atm to insert in the Alar3

        /// <summary>
        /// Initializes a new instance of the <see cref="Demo2Alar3"/> class.
        /// </summary>
        /// <param name="pngs">PNGs to insert.</param>
        /// <param name="digName">Name of the Dig.</param>
        /// <param name="atmNames">Name of the atm.</param>
        /// <param name="insertTransparent">Label to add a transparent pixel in the image.</param>
        public Demo2Alar3(Node[] pngs, string digName, string[] atmNames, bool insertTransparent)
        {
            Images = pngs;
            DigName = digName;
            AtmNames = atmNames;
            TransparentTile = insertTransparent;
        }

        /// <summary>
        /// Gets or sets the PNG we are inserting.
        /// </summary>
        public Node[] Images { get; set; }

        /// <summary>
        /// Gets or sets the original name of the Dig of the image.
        /// </summary>
        public string DigName { get; set; }

        /// <summary>
        /// Gets or sets the original name of the Atm of the image.
        /// </summary>
        public string[] AtmNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transparent pixel mode is enabled.
        /// </summary>
        public bool TransparentTile { get; set; }

        /// <summary>
        /// Converts a <see cref="Node"/> (png file) to a <see cref="Alar"/> container.
        /// </summary>
        /// <param name="originalAlar">Original Alar3.</param>
        /// <returns><see cref="Alar"/>Alar3 with the PNG inserted.</returns>
        public Alar Convert(Alar originalAlar)
        {
            if (Images.Length != AtmNames.Length) {
                throw new FormatException("Number of input PNGs does not match number of provided ATMs.");
            }

            transformedFiles = new NodeContainerFormat();

            // Obtaining the original Dig and Altms
            Node dig = Navigator.IterateNodes(originalAlar.Root).First(n => n.Name == DigName) ?? throw new FormatException("Dig doesn't exist: " + DigName);
            Node atmFull = Navigator.IterateNodes(originalAlar.Root).First(n => n.Name == AtmNames[0]) ?? throw new FormatException("Atm doesn't exist: " + AtmNames[0]);
            Node atmM = Navigator.IterateNodes(originalAlar.Root).First(n => n.Name == AtmNames[1]) ?? throw new FormatException("Atm doesn't exist: " + AtmNames[1]);
            Node atmN = Navigator.IterateNodes(originalAlar.Root).First(n => n.Name == AtmNames[2]) ?? throw new FormatException("Atm doesn't exist: " + AtmNames[2]);

            // Clone the nodes
            var dig_clone = (BinaryFormat)new BinaryFormat(dig.Stream!).DeepClone();
            var atmFull_clone = (BinaryFormat)new BinaryFormat(atmFull.Stream!).DeepClone();
            var atmM_clone = (BinaryFormat)new BinaryFormat(atmM.Stream!).DeepClone();
            var atmN_clone = (BinaryFormat)new BinaryFormat(atmN.Stream!).DeepClone();

            Node[] atms = [new Node(atmFull.Name, atmFull_clone), new Node(atmM.Name, atmM_clone), new Node(atmN.Name, atmN_clone)];

            Transform(Images, new Node(dig.Name, dig_clone), atms);

            originalAlar.InsertModification(transformedFiles);

            return originalAlar;
        }

        private void Transform(Node[] pngs, Node dig, Node[] atms)
        {
            // Original Dig
            bool digIsCompressed = CompressionUtils.IsCompressed(dig);
            Node uncompressedDig = digIsCompressed ?
                dig.TransformWith<LzssDecompression>() :
                dig;

            Dig mergedImage = uncompressedDig
                .TransformWith<Binary2Dig>()
                .GetFormatAs<Dig>() ?? throw new FormatException("Invalid dig file");

            // Transform PNG into a RgbImage (Pixels + Map) using the Dig Palette
            var compressionParams = new RgbImageMapCompressionParams {
                Palettes = mergedImage,
            };

            IndexedImage? newImage = null;

            // 2 - Iterate the input PNGs
            for (int i = 0; i < pngs.Length; i++) {
                if (Path.GetExtension(pngs[i].Name) != ".png") {
                    throw new FormatException("Invalid png file");
                }

                // Transform the PNG into RgbImage (Pixels + Map) using the palette of the original DIG
                pngs[i].Stream!.Position = 0;
                MapCompressedIndexedImage compressed = pngs[i].TransformWith<StandardBinaryImage2RgbImage>()
                    .TransformWith(new RgbImageMapCompression(compressionParams))
                    .GetFormatAs<MapCompressedIndexedImage>()!;

                newImage = new IndexedImage {
                    Width = 8,
                    Height = compressed.Tiles.Length / 8,
                    Pixels = compressed.Tiles,
                };

                ITileMap map = compressed.Map;

                // 3 - Clone original
                mergedImage = new Dig(mergedImage, newImage);

                if (TransparentTile && i == 0) {
                    mergedImage = mergedImage.InsertTransparentTile(map!);
                }

                compressionParams = new RgbImageMapCompressionParams {
                    MergeImage = mergedImage,
                    Palettes = mergedImage,
                };

                // Original Atm
                bool atmIsCompressed = CompressionUtils.IsCompressed(atms[i]);
                Node uncompressedAtm = atmIsCompressed ?
                    atms[i].TransformWith<LzssDecompression>() :
                    atms[i];

                // New Atm: original atm changing height, width and maps
                Altm originalAtm = uncompressedAtm
                        .TransformWith<Binary2Altm>()
                        .GetFormatAs<Altm>() ?? throw new FormatException("Invalid atm file");

                var newAtm = new Altm(originalAtm, map!);

                // Export ATM
                BinaryFormat binaryAtm = new Altm2Binary().Convert(newAtm);

                BinaryFormat compressedAtm = atmIsCompressed ?
                    new LzssCompression().Convert(binaryAtm) :
                    binaryAtm;

                transformedFiles.Root.Add(new Node(atms[i].Name, compressedAtm));
            }

            // New Dig: original dig changing height, width and pixels
            var newDig = new Dig(mergedImage, newImage!);

            BinaryFormat binaryDig = new Dig2Binary().Convert(newDig);

            BinaryFormat compressedDig = digIsCompressed ?
                new LzssCompression().Convert(binaryDig) :
                binaryDig;

            transformedFiles.Root.Add(new Node(dig.Name, compressedDig));
        }
    }
}
