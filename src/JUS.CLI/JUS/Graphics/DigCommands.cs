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
using JUS.Tool.Graphics;
using JUS.Tool.Graphics.Converters;
using JUS.Tool.Utils;
using Texim.Formats.ImageSharp.Images;
using Texim.Images;
using Texim.TileMaps;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.CLI.JUS.Graphics
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

            pixelsPaletteNode.Stream.WriteTo(Path.Combine(output, Path.GetFileNameWithoutExtension(mapsNode.Name) + ".png"));

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

            Node originalDig = NodeFactory.FromFile(dig, FileOpenMode.Read);
            Node originalAtm = NodeFactory.FromFile(atm, FileOpenMode.Read);
            Node png = NodeFactory.FromFile(input, FileOpenMode.Read);

            var converter = new Png2DigAtm(originalDig, originalAtm, insertTransparent);
            NodeContainerFormat transformedFiles = converter.Convert(png);

            transformedFiles.Root.Children[originalDig.Name]!.Stream!.WriteTo(
                Path.Combine(output, Path.GetFileNameWithoutExtension(input) + ".dig"));
            transformedFiles.Root.Children[originalAtm.Name]!.Stream!.WriteTo(
                Path.Combine(output, Path.GetFileNameWithoutExtension(input) + ".atm"));

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
            if (input.Length != atm.Length)
            {
                throw new FormatException("Number of input PNGs does not match number of provided ATMs.");
            }

            // 1 - Get the DIG
            Dig mergedImage = NodeFactory.FromFile(dig)
                .TransformWith<LzssDecompression>()
                .TransformWith<Binary2Dig>()
                .GetFormatAs<Dig>();

            var compressionParams = new RgbImageMapCompressionParams
            {
                Palettes = mergedImage,
            };

            IndexedImage? newImage = null;

            // 2 - Iterate the input PNGs
            for (int i = 0; i < input.Length; i++)
            {
                // Transform the PNG into RgbImage (Pixels + Map) using the palette of the original DIG
                MapCompressedIndexedImage compressed = NodeFactory.FromFile(input[i], FileOpenMode.Read)
                    .TransformWith<StandardBinaryImage2RgbImage>()
                    .TransformWith(new RgbImageMapCompression(compressionParams))
                    .GetFormatAs<MapCompressedIndexedImage>();

                newImage = new IndexedImage
                {
                    Width = 8,
                    Height = compressed.Tiles.Length / 8,
                    Pixels = compressed.Tiles,
                };
                ITileMap map = compressed.Map;

                // 3 - Clone original
                mergedImage = new Dig(mergedImage, newImage);

                if (insertTransparent && i == 0)
                {
                    mergedImage = mergedImage.InsertTransparentTile(map);
                }

                compressionParams = new RgbImageMapCompressionParams
                {
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
            var newDig = new Dig(mergedImage, newImage!);
            new Dig2Binary().Convert(newDig)
                .Stream.WriteTo(Path.Combine(output, Path.GetFileName(dig)));

            Console.WriteLine("Done!");
        }
    }
}
