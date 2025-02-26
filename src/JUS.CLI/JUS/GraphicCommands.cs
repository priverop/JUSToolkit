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
using System.Collections.Generic;
using System.IO;
using JUS.Tool.Graphics.Converters;
using JUSToolkit.Containers.Converters;
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
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
                .TransformWith<Dtx2Bitmaps>();

            foreach (Node nodeSprite in dtx3.Children) {
                nodeSprite.Stream.WriteTo(Path.Combine(output, $"{nodeSprite.Name}.png"));
            }
        }

        /// <summary>
        /// Import multiple PNGs into a sprite .dtx file.
        /// </summary>
        /// <param name="input">The input folder containing PNGs.</param>
        /// <param name="dtx">The .dtx file.</param>
        /// <param name="output">The output folder.</param>
        public static void ImportDtx3(string input, string dtx, string output)
        {
            // Sprites + pixels + palette
            using Node dtx3 = NodeFactory.FromFile(dtx, FileOpenMode.Read)
                .TransformWith<LzssDecompression>()
                .TransformWith<BinaryToDtx3>();

            Dig image = dtx3.Children["image"].GetFormatAs<Dig>();
            var palettes = new PaletteCollection();
            foreach (IPalette p in image.Palettes) {
                palettes.Palettes.Add(p);
            }

            var pixels = new List<IndexedPixel>();

            var spriteConverterParameters = new FullImage2SpriteParams {
                Palettes = palettes,
                IsImageTiled = true,
                MinimumPixelsPerSegment = 64,
                PixelsPerIndex = 64,
                RelativeCoordinates = SpriteRelativeCoordinatesKind.Center,
                PixelSequences = pixels,
                Segmentation = new NitroImageSegmentation(),
            };

            foreach (string spritePath in Directory.GetFiles(input)) {
                Node nodeSprite = NodeFactory.FromFile(spritePath, FileOpenMode.Read);
                // PNG -> FullImage (array of colors)
                nodeSprite.TransformWith<Bitmap2FullImage>();
                // FullImage -> Sprite
                var converter = new FullImage2Sprite(spriteConverterParameters);
                nodeSprite.TransformWith(converter);
                Sprite sprite = nodeSprite.GetFormatAs<Sprite>();

                // Check if there is a Children with the correct name:
                string cleanSpriteName = Path.GetFileNameWithoutExtension(spritePath);
                Node spriteToReplace = dtx3.Children["sprites"].Children[cleanSpriteName]
                ?? throw new ArgumentException($"Wrong sprite name: {cleanSpriteName}");

                spriteToReplace.ChangeFormat(sprite);
            }

            var updatedImage = new Dig(image) {
                Pixels = pixels.ToArray(),
                Width = 8,
                Height = pixels.Count / 8,
            };

            dtx3.Children["image"].ChangeFormat(updatedImage);

            new Dtx3ToBinary().Convert(dtx3.GetFormatAs<NodeContainerFormat>())
                .Stream.WriteTo(Path.Combine(output, "file.dtx"));

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

                _ = dtx.TransformWith<BinaryDstx2SpriteImage>();
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
