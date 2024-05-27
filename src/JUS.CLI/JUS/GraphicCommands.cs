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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using JUS.Tool.Graphics.Converters;
using JUSToolkit.Containers.Converters;
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using SixLabors.ImageSharp.PixelFormats;
using Texim.Compressions.Nitro;
using Texim.Formats;
using Texim.Images;
using Texim.Palettes;
using Texim.Pixels;
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
            using Node mapsNode = NodeFactory.FromFile(atm, FileOpenMode.Read);

            var binaryDig2Bitmap = new BinaryDig2Bitmap(mapsNode);

            using Node pixelsPaletteNode = NodeFactory.FromFile(dig, FileOpenMode.Read)
                .TransformWith(binaryDig2Bitmap);

            pixelsPaletteNode.Stream.WriteTo(output + ".png");

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Export a sprite .dtx file into multiple PNGs.
        /// </summary>
        /// <param name="dtx">The .dtx file.</param>
        /// <param name="output">The output folder.</param>
        /// <exception cref="FormatException"><paramref name="dtx"/> file doesn't have a valid format.</exception>
        public static void ExportDtx3(string dtx, string output)
        {
            // Sprites + pixels + palette
            using Node dtx3 = NodeFactory.FromFile(dtx, FileOpenMode.Read)
                .TransformWith<LzssDecompression>()
                .TransformWith<BinaryToDtx3>();

            Dig image = dtx3.Children["image"].GetFormatAs<Dig>();
            var spriteParams = new Sprite2IndexedImageParams {
                RelativeCoordinates = SpriteRelativeCoordinatesKind.Center,
                FullImage = image,
            };
            var indexedImageParams = new IndexedImageBitmapParams {
                Palettes = image,
            };

            switch (image.Swizzling) {
                case DigSwizzling.Tiled:
                    foreach (Node nodeSprite in dtx3.Children["sprites"].Children) {
                        nodeSprite
                            .TransformWith(new Sprite2IndexedImage(spriteParams))
                            .TransformWith(new IndexedImage2Bitmap(indexedImageParams))
                            .Stream.WriteTo(Path.Combine(output, $"{nodeSprite.Name}.png"));
                    }

                    break;
                case DigSwizzling.Linear:
                    foreach (Node nodeTexture in dtx3.Children["sprites"].Children) {
                        nodeTexture
                            .TransformWith(new IndexedImage2Bitmap(indexedImageParams))
                            .Stream.WriteTo(Path.Combine(output, $"{nodeTexture.Name}.png"));
                    }

                    break;
                default:
                    throw new FormatException("Invalid swizzling");
            }
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
            Dig originalDig = NodeFactory.FromFile(dig)
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

            using var binaryDig = new Dig2Binary().Convert(newDig);

            binaryDig.Stream.WriteTo(Path.Combine(output, Path.GetFileNameWithoutExtension(input) + ".dig"));

            var newAtm = new Almt(originalAtm, map);
            using var binaryAtm = new Almt2Binary().Convert(newAtm);

            binaryAtm.Stream.WriteTo(Path.Combine(output, Path.GetFileNameWithoutExtension(input) + ".atm"));

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Import multiple PNGs into multiple ATMs that share the same DIG.
        /// </summary>
        /// <param name="input">The pngs to import.</param>
        /// <param name="insertTransparent">Insert a transparent tile at the start of the image.</param>
        /// <param name="dig">The original .dig file.</param>
        /// <param name="atm">The original .atm files.</param>
        /// <param name="output">The output folder.</param>
        public static void MergeDig(string[] input, bool insertTransparent, string dig, string[] atm, string output)
        {
            if (input.Length != atm.Length)
                throw new FormatException("Number of input PNGs does not match number of provided ATMs.");

            Dig mergedImage = NodeFactory.FromFile(dig)
                .TransformWith<LzssDecompression>()
                .TransformWith<Binary2Dig>()
                .GetFormatAs<Dig>();

            var compressionParams = new FullImageMapCompressionParams {
                Palettes = mergedImage,
            };

            IndexedImage newImage = null;

            for (int i = 0; i < input.Length; i++) {
                Node compressed = NodeFactory.FromFile(input[i], FileOpenMode.Read)
                .TransformWith<Bitmap2FullImage>()
                .TransformWith(new FullImageMapCompression(compressionParams));
                newImage = compressed.Children[0].GetFormatAs<IndexedImage>();
                ScreenMap map = compressed.Children[1].GetFormatAs<ScreenMap>();

                mergedImage = new Dig(mergedImage, newImage);

                if (insertTransparent && i == 0)
                    mergedImage = mergedImage.InsertTransparentTile(map);

                compressionParams = new FullImageMapCompressionParams {
                    MergeImage = mergedImage,
                    Palettes = mergedImage,
                };

                Almt originalAtm = NodeFactory.FromFile(atm[i], FileOpenMode.Read)
                    .TransformWith<Binary2Almt>()
                    .GetFormatAs<Almt>();
                var newAtm = new Almt(originalAtm, map);

                new Almt2Binary().Convert(newAtm)
                    .Stream.WriteTo(Path.Combine(output, Path.GetFileName(atm[i])));
            }

            Dig newDig = new Dig(mergedImage, newImage);
            new Dig2Binary().Convert(newDig)
                .Stream.WriteTo(Path.Combine(output, Path.GetFileName(dig)));

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
                .Children["koma"] ?? throw new FormatException("Invalid container file");

            KShapeSprites shapes = NodeFactory.FromFile(kshape)
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
                IndexedPaletteImage image = dtx.Children["image"].GetFormatAs<IndexedPaletteImage>();

                // We ignore the sprite info from the DSTX and we take the one
                // from the kshape
                Sprite sprite = shapes.GetSprite(komaElement.KShapeGroupId, komaElement.KShapeElementId);

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
                    .TransformWith(new Sprite2IndexedImage(spriteParams))
                    .TransformWith(new IndexedImage2Bitmap(indexedImageParams))
                    .Stream.WriteTo(outputFilePath);
            }

            Console.WriteLine("Done!");
        }
    }
}
