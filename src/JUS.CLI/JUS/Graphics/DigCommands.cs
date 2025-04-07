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
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using Texim.Compressions.Nitro;
using Texim.Formats;
using Texim.Images;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.CLI.JUS
{
    /// <summary>
    /// Commands related to DSIG/DIG graphics files.
    /// </summary>
    public static class DigCommands
    {
        /// <summary>
        /// Export a DSIG + ALMT into a PNG.
        /// </summary>
        /// <param name="dig">The file.dig.</param>
        /// <param name="atm">The map.atm file.</param>
        /// <param name="output">The output folder.</param>
        public static void ExportDig(string dig, string atm, string output)
        {
            using Node mapsNode = NodeFactory.FromFile(atm, FileOpenMode.Read);

            var binaryDig2Bitmap = new BinaryDig2Bitmap(mapsNode);

            using Node pixelsPaletteNode = NodeFactory.FromFile(dig, FileOpenMode.Read)
                .TransformWith(binaryDig2Bitmap);

            pixelsPaletteNode.Stream.WriteTo(output + ".png");

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Import a PNG into a DSIG + ALMT.
        /// </summary>
        /// <param name="input">The png to import.</param>
        /// <param name="insertTransparent">Insert a transparent tile at the start of the image.</param>
        /// <param name="dig">The original .dig file.</param>
        /// <param name="atm">The original .atm file.</param>
        /// <param name="output">The output folder.</param>
        public static void ImportDig(string input, bool insertTransparent, string dig, string atm, string output)
        {
            Console.WriteLine(input);
            Console.WriteLine(dig);
            Console.WriteLine(atm);

            Dig originalDig = NodeFactory.FromFile(dig, FileOpenMode.Read)
                .TransformWith<LzssDecompression>()
                .TransformWith<Binary2Dig>()
                .GetFormatAs<Dig>();

            Almt originalAtm = NodeFactory.FromFile(atm, FileOpenMode.Read)
                .TransformWith<LzssDecompression>()
                .TransformWith<Binary2Almt>()
                .GetFormatAs<Almt>();

            if (originalDig is null) {
                throw new FormatException("Invalid dig file");
            }

            if (originalAtm is null) {
                throw new FormatException("Invalid atm file");
            }

            var compressionParams = new FullImageMapCompressionParams {
                Palettes = originalDig,
            };

            Node compressed = NodeFactory.FromFile(input, FileOpenMode.Read)
                .TransformWith<Bitmap2FullImage>()
                .TransformWith(new FullImageMapCompression(compressionParams));
            IndexedImage newImage = compressed.Children[0].GetFormatAs<IndexedImage>();
            ScreenMap map = compressed.Children[1].GetFormatAs<ScreenMap>();

            var newDig = new Dig(originalDig, newImage);

            if (insertTransparent) {
                newDig = newDig.InsertTransparentTile(map);
            }

            using BinaryFormat binaryDig = new Dig2Binary().Convert(newDig);

            binaryDig.Stream.WriteTo(Path.Combine(output, Path.GetFileNameWithoutExtension(input) + ".dig"));

            var newAtm = new Almt(originalAtm, map);
            using BinaryFormat binaryAtm = new Almt2Binary().Convert(newAtm);

            binaryAtm.Stream.WriteTo(Path.Combine(output, Path.GetFileNameWithoutExtension(input) + ".atm"));

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Import multiple PNGs into multiple ATMs that share the same DIG. The result is multiple atm and a single dig.
        /// </summary>
        /// <param name="input">The pngs to import.</param>
        /// <param name="insertTransparent">Insert a transparent tile at the start of the image.</param>
        /// <param name="dig">The original .dig file (merged image).</param>
        /// <param name="atm">The original .atm files.</param>
        /// <param name="output">The output folder.</param>
        public static void MergeDig(string[] input, bool insertTransparent, string dig, string[] atm, string output)
        {
            if (input.Length != atm.Length) {
                throw new FormatException("Number of input PNGs does not match number of provided ATMs.");
            }

            // 1 - Get the DIG
            Dig mergedImage = NodeFactory.FromFile(dig)
                .TransformWith<LzssDecompression>()
                .TransformWith<Binary2Dig>()
                .GetFormatAs<Dig>();

            var compressionParams = new FullImageMapCompressionParams {
                Palettes = mergedImage,
            };

            IndexedImage newImage = null;

            // 2 - Iterate the input PNGs
            for (int i = 0; i < input.Length; i++) {
                // Transform the PNG into FullImage (Pixels + Map) using the palette of the original DIG
                Node png = NodeFactory.FromFile(input[i], FileOpenMode.Read)
                    .TransformWith<Bitmap2FullImage>()
                    .TransformWith(new FullImageMapCompression(compressionParams));

                // Pixels
                newImage = png.Children[0].GetFormatAs<IndexedImage>();

                // Map
                ScreenMap map = png.Children[1].GetFormatAs<ScreenMap>();

                // 3 - Clone original
                mergedImage = new Dig(mergedImage, newImage);

                if (insertTransparent && i == 0) {
                    mergedImage = mergedImage.InsertTransparentTile(map);
                }

                compressionParams = new FullImageMapCompressionParams {
                    MergeImage = mergedImage,
                    Palettes = mergedImage,
                };

                // New Atm: original atm changing height, width and maps
                Almt originalAtm = NodeFactory.FromFile(atm[i], FileOpenMode.Read)
                    .TransformWith<Binary2Almt>()
                    .GetFormatAs<Almt>();
                var newAtm = new Almt(originalAtm, map);

                // Export ATM
                new Almt2Binary().Convert(newAtm)
                    .Stream.WriteTo(Path.Combine(output, Path.GetFileName(atm[i])));
            }

            // New Dig: original dig changing height, width and pixels
            var newDig = new Dig(mergedImage, newImage);
            new Dig2Binary().Convert(newDig)
                .Stream.WriteTo(Path.Combine(output, Path.GetFileName(dig)));

            Console.WriteLine("Done!");
        }
    }
}
