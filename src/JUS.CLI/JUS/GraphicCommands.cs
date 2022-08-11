// Copyright (c) 2022 Pablo Rivero

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
using JUSToolkit.Containers.Converters;
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using Texim.Compressions.Nitro;
using Texim.Formats;
using Texim.Images;
using Texim.Palettes;
using Texim.Sprites;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.CLI.JUS
{
    /// <summary>
    /// Commands related to graphics files.
    /// </summary>
    public static class GraphicCommands
    {
        /// <summary>
        /// Export a DSIG + ALMT into a PNG.
        /// </summary>
        /// <param name="dig">The file.dig.</param>
        /// <param name="atm">The map.atm file.</param>
        /// <param name="output">The output folder.</param>
        public static void ExportDig(string dig, string atm, string output)
        {
            // Pixels + Palette
            using var pixelsPaletteNode = NodeFactory.FromFile(dig, FileOpenMode.Read)
                .TransformWith<LzssDecompression>()
                .TransformWith<BinaryDsig2IndexedPaletteImage>();

            // Map
            using var mapsNode = NodeFactory.FromFile(atm, FileOpenMode.Read)
                .TransformWith<LzssDecompression>()
                .TransformWith<Binary2Almt>();

            var mapsParams = new MapDecompressionParams {
                Map = mapsNode.GetFormatAs<Almt>(),
                TileSize = mapsNode.GetFormatAs<Almt>().TileSize,
            };
            var bitmapParams = new IndexedImageBitmapParams {
                Palettes = pixelsPaletteNode.GetFormatAs<IndexedPaletteImage>(),
            };
            pixelsPaletteNode.TransformWith<MapDecompression, MapDecompressionParams>(mapsParams)
                .TransformWith<IndexedImage2Bitmap, IndexedImageBitmapParams>(bitmapParams)
                .Stream.WriteTo(output + "export.png");

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Import a PNG into a DSIG + ALMT.
        /// </summary>
        /// <param name="dig">The file.dig.</param>
        /// <param name="atm">The map.atm file.</param>
        /// <param name="output">The output folder.</param>
        public static void ImportDig(string dig, string atm, string output)
        {
            Node originalDig = NodeFactory.FromFile(dig)
                .TransformWith<Binary2Dig>();

            if (originalDig is null) {
                throw new FormatException("Invalid dig file");
            }

            originalDig.TransformWith<Dig2Binary>();

            originalDig.Stream.WriteTo(Path.Combine(output, "imported_" + originalDig.Name));

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Export a DTX into PNG komas.
        /// </summary>
        /// <param name="container">The koma.aar container.</param>
        /// <param name="koma">The koma.bin file.</param>
        /// <param name="kshape">The kshape.bin file.</param>
        /// <param name="output">The output folder.</param>
        public static void ExportDtx(string container, string koma, string kshape, string output)
        {
            Node images = NodeFactory.FromFile(container)
                .TransformWith<Binary2Alar3>()
                .Children["koma"];
            if (images is null) {
                throw new FormatException("Invalid container file");
            }

            var shapes = NodeFactory.FromFile(kshape)
                .TransformWith<BinaryKShape2SpriteCollection>()
                .GetFormatAs<KShapeSprites>();

            Koma komaFormat = NodeFactory.FromFile(koma)
                .TransformWith<Binary2Koma>()
                .GetFormatAs<Koma>();
            foreach (KomaElement komaElement in komaFormat) {
                string filename = $"{komaElement.KomaName}.dtx";

                Node dtx = images.Children[filename];
                if (dtx is null) {
                    Console.WriteLine("- Missing: " + filename);
                    continue;
                }

                dtx.TransformWith<BinaryDstx2SpriteImage>();
                var image = dtx.Children["image"].GetFormatAs<IndexedPaletteImage>();

                // We ignore the sprite info from the DSTX and we take the one
                // from the kshape
                var sprite = shapes.GetSprite(komaElement.KShapeGroupId, komaElement.KShapeElementId);

                string outputFilePath = Path.Combine(
                    output,
                    $"{komaElement.KShapeGroupId}",
                    komaElement.KomaName + ".png");

                var spriteParams = new Sprite2IndexedImageParams {
                    RelativeCoordinates = SpriteRelativeCoordinatesKind.TopLeft,
                    FullImage = image,
                };
                var indexedImageParams = new IndexedImageBitmapParams {
                   Palettes = image,
                };
                new Node("sprite", sprite)
                    .TransformWith<Sprite2IndexedImage, Sprite2IndexedImageParams>(spriteParams)
                    .TransformWith<IndexedImage2Bitmap, IndexedImageBitmapParams>(indexedImageParams)
                    .Stream.WriteTo(outputFilePath);
            }

            Console.WriteLine("Done!");
        }
    }
}
