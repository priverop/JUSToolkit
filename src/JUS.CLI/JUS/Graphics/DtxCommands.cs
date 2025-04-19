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
using System.Drawing;
using System.IO;
using System.Linq;
using JUS.Tool.Graphics.Converters;
using JUSToolkit.Containers.Converters;
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using Texim.Compressions.Nitro;
using Texim.Formats;
using Texim.Images;
using Texim.Palettes;
using Texim.Pixels;
using Texim.Processing;
using Texim.Sprites;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.CLI.JUS
{
    /// <summary>
    /// Commands related to Sprites (dtx) graphics files.
    /// </summary>
    public static class DtxCommands
    {
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
        /// <param name="dtx">The original .dtx file.</param>
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

            var segmentation = new NitroImageSegmentation() {
                CanvasWidth = 256,
                CanvasHeight = 256,
            };
            var spriteConverterParameters = new FullImage2SpriteParams {
                Palettes = palettes,
                IsImageTiled = true,
                MinimumPixelsPerSegment = 64,
                PixelsPerIndex = 64,
                RelativeCoordinates = SpriteRelativeCoordinatesKind.Center,
                PixelSequences = pixels,
                Segmentation = segmentation,
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
                .Stream.WriteTo(Path.Combine(output, Path.GetFileName(dtx)));

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Export a DTX into PNG komas.
        /// </summary>
        /// <param name="container">The koma.aar container.</param>
        /// <param name="koma">The koma.bin file.</param>
        /// <param name="kshape">The kshape.bin file.</param>
        /// <param name="output">The output folder.</param>
        public static void ExportKomas(string container, string koma, string kshape, string output)
        {
            Node komas = NodeFactory.FromFile(container)
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

                Node dtx = komas.Children[filename];
                if (dtx is null) {
                    Console.WriteLine("- Missing: " + filename);
                    continue;
                }

                _ = dtx.TransformWith<BinaryDtx4ToSpriteImage>();
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

        /// <summary>
        /// Export a sprite .dtx of type 04 into a PNG.
        /// </summary>
        /// <param name="dtx">The .dtx file.</param>
        /// <param name="koma">The koma.bin file.</param>
        /// <param name="kshape">The kshape.bin file.</param>
        /// <param name="output">The output folder.</param>
        /// <exception cref="FormatException"><paramref name="dtx"/> file doesn't have a valid format.</exception>
        public static void ExportDtx4(string dtx, string koma, string kshape, string output)
        {
            // Sprites + pixels + palette
            using Node dtx4 = NodeFactory.FromFile(dtx, FileOpenMode.Read)
            .TransformWith<LzssDecompression>()
            .TransformWith<BinaryDtx4ToSpriteImage>(); // NCF with sprite+image

            Sprite sprite2 = dtx4.Children["sprite"].GetFormatAs<Sprite>();
            IndexedPaletteImage image = dtx4.Children["image"].GetFormatAs<IndexedPaletteImage>();

            KShapeSprites shapes = NodeFactory.FromFile(kshape)
                .TransformWith<BinaryKShape2SpriteCollection>()
                .GetFormatAs<KShapeSprites>();

            Koma komaFormat = NodeFactory.FromFile(koma)
                .TransformWith<Binary2Koma>()
                .GetFormatAs<Koma>();

            KomaElement komaElement = komaFormat.First(n => n.KomaName == Path.GetFileNameWithoutExtension(dtx)) ?? throw new FormatException("Can't find the dtx in the koma.bin");

            // We ignore the sprite info from the DSTX and we take the one
            // from the kshape
            Sprite sprite = shapes.GetSprite(komaElement.KShapeGroupId, komaElement.KShapeElementId);

            var spriteParams = new Sprite2IndexedImageParams {
                RelativeCoordinates = SpriteRelativeCoordinatesKind.TopLeft,
                FullImage = image,
            };
            var indexedImageParams = new IndexedImageBitmapParams {
                Palettes = image,
            };

            // Debugging:
            // Export all the segments
            ImageSegment2IndexedImage segment2Indexed = new ImageSegment2IndexedImage(new ImageSegment2IndexedImageParams {
                FullImage = image,
                IsTiled = true,
            });
            int i = 0;
            foreach (IImageSegment segment in sprite.Segments) {
                FullImage originalSegmentImage = segment2Indexed.Convert(segment).CreateFullImage(image, true);
                BinaryFormat pngSegment = new FullImage2Bitmap().Convert(originalSegmentImage);
                pngSegment.Stream.WriteTo(Path.Combine(output, $"segment_kshape{i}.png"));
                i++;
            }
            int j = 0;
            foreach (IImageSegment segment in sprite2.Segments) {
                FullImage originalSegmentImage = segment2Indexed.Convert(segment).CreateFullImage(image, true);
                BinaryFormat pngSegment = new FullImage2Bitmap().Convert(originalSegmentImage);
                pngSegment.Stream.WriteTo(Path.Combine(output, $"segment_dtx{j}.png"));
                j++;
            }
            // -------------------

            new Node("sprite", sprite)
                .TransformWith(new Sprite2IndexedImage(spriteParams))
                .TransformWith(new IndexedImage2Bitmap(indexedImageParams))
                .Stream.WriteTo(Path.Combine(output, Path.GetFileNameWithoutExtension(dtx) + ".png"));

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Import a PNG into a koma .dtx file.
        /// </summary>
        /// <param name="png">The png to import.</param>
        /// <param name="dtx">The .dtx koma file.</param>
        /// <param name="koma">The koma.bin file (links the .dtx name to the kshape id).</param>
        /// <param name="kshape">The kshape.bin file (actual sprite info of the dtx).</param>
        /// <param name="output">The output folder.</param>
        public static void ImportKoma(string png, string dtx, string koma, string kshape, string output)
        {
            // First we get the original dtx
            using Node dtx4 = NodeFactory.FromFile(dtx, FileOpenMode.Read)
                .TransformWith<LzssDecompression>()
                .TransformWith<BinaryDtx4ToSpriteImage>(); // NCF with sprite+image

            // Get the image (dig) of the dtx
            Dig originalImage = dtx4.Children["image"].GetFormatAs<Dig>();

            // For debugging:
            var a = new Dig2Binary().Convert(originalImage);
            a.Stream.WriteTo(Path.Combine(output, "original.dig"));
            // -------------------

            var palettes = new PaletteCollection();
            foreach (IPalette p in originalImage.Palettes) {
                palettes.Palettes.Add(p);
            }

            // var newPixels = new List<IndexedPixel>();

            // // Prepare the SpriteParams with the palette of the image
            // var segmentation = new NitroImageSegmentation() {
            //     CanvasWidth = 240,
            //     CanvasHeight = 192,
            // };
            // var spriteConverterParameters = new FullImage2SpriteParams {
            //     Palettes = palettes,
            //     IsImageTiled = true,
            //     MinimumPixelsPerSegment = 64,
            //     PixelsPerIndex = 64,
            //     RelativeCoordinates = SpriteRelativeCoordinatesKind.Center,
            //     PixelSequences = newPixels,
            //     Segmentation = segmentation,
            // };

            // PNG -> Sprite:
            Node pngNode = NodeFactory.FromFile(png, FileOpenMode.Read);

            // PNG -> IndexedImage
            var quantization = new FixedPaletteQuantization(originalImage.Palettes[0]);
            pngNode.TransformWith<Bitmap2FullImage>().TransformWith(new FullImage2IndexedPalette(quantization));
            IndexedPaletteImage newImage = pngNode.GetFormatAs<IndexedPaletteImage>();
            IndexedPixel[] tiledPixels = new TileSwizzling<IndexedPixel>(48).Swizzle(newImage.Pixels);

            // Sprite from KShape
            KShapeSprites shapes = NodeFactory.FromFile(kshape)
                .TransformWith<BinaryKShape2SpriteCollection>()
                .GetFormatAs<KShapeSprites>();

            Koma komaFormat = NodeFactory.FromFile(koma)
                .TransformWith<Binary2Koma>()
                .GetFormatAs<Koma>();

            KomaElement komaElement = komaFormat.First(n => n.KomaName == Path.GetFileNameWithoutExtension(dtx)) ?? throw new FormatException("Can't find the dtx in the koma.bin");

            // We ignore the sprite info from the DSTX and we take the one
            // from the kshape
            Sprite sprite = shapes.GetSprite(komaElement.KShapeGroupId, komaElement.KShapeElementId);


            // FullImage -> Sprite
            // var converter = new FullImage2Sprite(spriteConverterParameters);
            // pngNode.TransformWith(converter);
            // Sprite newSprite = pngNode.GetFormatAs<Sprite>();

            // Debugging:
            // Exporting the new image
            // Debugging: Exporting back the IndexedImage to Bitmap to check if it worked ok
            // pngNode.TransformWith(new IndexedImage2Bitmap(new IndexedImageBitmapParams() {
            //     Palettes = palettes,
            // }));
            // pngNode.Stream.WriteTo(Path.Combine(output, "newImage.png"));
            // Export all the segments with the new image
            // ImageSegment2IndexedImage segment2Indexed = new ImageSegment2IndexedImage(new ImageSegment2IndexedImageParams {
            //     FullImage = newImage,
            //     IsTiled = false,
            // });
            // int i = 0;
            // foreach (IImageSegment segment in sprite.Segments) {
            //     segment.TileIndex--;
            //     FullImage originalSegmentImage = segment2Indexed.Convert(segment).CreateFullImage(palettes, false);
            //     BinaryFormat pngSegment = new FullImage2Bitmap().Convert(originalSegmentImage);
            //     pngSegment.Stream.WriteTo(Path.Combine(output, $"segment{i}.png"));
            //     i++;
            // }
            // -------------------

            // Replace original Sprite with the new one
            // Node spriteToReplace = dtx4.Children["sprite"] ?? throw new ArgumentException("Error getting sprite children");
            // spriteToReplace.ChangeFormat(newSprite);

            // // Update image with the new changes
            var updatedImage = new Dig(originalImage) {
                Pixels = tiledPixels,
                Width = 8,
                Height = newImage.Pixels.Length / 8,
                Swizzling = DigSwizzling.Linear,
            }.InsertTransparentTile();

            // // For debugging:
            var b = new Dig2Binary().Convert(updatedImage);
            b.Stream.WriteTo(Path.Combine(output, "updated.dig"));
            // // -------------------

            dtx4.Children["image"].ChangeFormat(updatedImage);

            // // Export the new .dtx
            new Dtx4ToBinary().Convert(dtx4.GetFormatAs<NodeContainerFormat>())
                .Stream.WriteTo(Path.Combine(output, Path.GetFileName(dtx)));

            Console.WriteLine("Done!");
        }
    }
}
