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
using Texim.Formats.ImageSharp.Images;
using Texim.TileMaps;
using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace JUS.Tool.BatchConverters
{
    /// <summary>
    /// Inserts a PNG into an Alar3.
    /// </summary>
    public class Demo2Alar3 :
        IConverter<Alar, Alar>
    {
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

            // Obtaining the original Dig and Almts
            Node dig = Navigator.IterateNodes(originalAlar.Root).FirstOrDefault(n => n.Name == DigName) ?? throw new FormatException("Dig doesn't exist: " + DigName);
            Node atmFull = Navigator.IterateNodes(originalAlar.Root).FirstOrDefault(n => n.Name == AtmNames[0]) ?? throw new FormatException("Atm doesn't exist: " + AtmNames[0]);
            Node atmM = Navigator.IterateNodes(originalAlar.Root).FirstOrDefault(n => n.Name == AtmNames[1]) ?? throw new FormatException("Atm doesn't exist: " + AtmNames[1]);
            Node atmN = Navigator.IterateNodes(originalAlar.Root).FirstOrDefault(n => n.Name == AtmNames[2]) ?? throw new FormatException("Atm doesn't exist: " + AtmNames[2]);

            Transform(Images, dig, [atmFull, atmM, atmN]);

            return originalAlar;
        }

        private void Transform(Node[] pngs, Node dig, Node[] atms)
        {
            // Original Dig
            bool digIsCompressed = CompressionUtils.IsCompressed(dig);
            if (digIsCompressed) {
                dig.TransformWith<LzssDecompression>();
            }

            dig.TransformWith<Binary2Dig>();
            Dig mergedImage = dig.GetFormatAs<Dig>() ?? throw new FormatException("Invalid dig file");

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
                pngs[i].Stream.Position = 0;
                MapCompressedIndexedImage compressed = pngs[i].TransformWith<StandardBinaryImage2RgbImage>()
                    .TransformWith(new RgbImageMapCompression(compressionParams))
                    .GetFormatAs<MapCompressedIndexedImage>();

                newImage = new IndexedImage {
                    Width = 8,
                    Height = compressed.Tiles.Length / 8,
                    Pixels = compressed.Tiles,
                };

                ITileMap map = compressed.Map;

                // 3 - Clone original
                mergedImage = new Dig(mergedImage, newImage);

                if (TransparentTile && i == 0) {
                    mergedImage = mergedImage.InsertTransparentTile(map);
                }

                compressionParams = new RgbImageMapCompressionParams {
                    MergeImage = mergedImage,
                    Palettes = mergedImage,
                };

                // Original Atm
                bool atmIsCompressed = CompressionUtils.IsCompressed(atms[i]);
                if (atmIsCompressed) {
                    atms[i].TransformWith<LzssDecompression>();
                }

                // New Atm: original atm changing height, width and maps
                atms[i].TransformWith<Binary2Almt>();
                Almt originalAtm = atms[i].GetFormatAs<Almt>() ?? throw new FormatException("Invalid atm file");

                var newAtm = new Almt(originalAtm, map);
                atms[i].ChangeFormat(newAtm);

                // Export ATM
                atms[i].TransformWith(new Almt2Binary());
                if (atmIsCompressed) {
                    atms[i].TransformWith<LzssCompression>();
                }
            }

            // New Dig: original dig changing height, width and pixels
            var newDig = new Dig(mergedImage, newImage!);
            dig.ChangeFormat(newDig)
                .TransformWith<Dig2Binary>();

            if (digIsCompressed) {
                dig.TransformWith<LzssCompression>();
            }
        }
    }
}
