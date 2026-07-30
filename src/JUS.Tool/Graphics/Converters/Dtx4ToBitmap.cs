// Copyright (c) 2026 Priverop

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
using Texim.Images;
using Texim.Images.Standard;
using Texim.Sprites;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Graphics.Converters
{
    /// <summary>
    /// Converts a IBinary (DTX4 file) into a BinaryFormat (PNG bitmap).
    /// </summary>
    public class Dtx4ToBitmap : IConverter<IBinary, BinaryFormat>
    {
        private readonly KShapeSprites shapes;
        private readonly Koma koma;
        private readonly string dtxName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dtx4ToBitmap"/> class.
        /// </summary>
        /// <param name="shapes">The KShape sprite collection.</param>
        /// <param name="koma">The Koma format with element mappings.</param>
        /// <param name="dtxName">The DTX file name (without extension) to look up in Koma.</param>
        public Dtx4ToBitmap(KShapeSprites shapes, Koma koma, string dtxName)
        {
            ArgumentNullException.ThrowIfNull(shapes);
            ArgumentNullException.ThrowIfNull(koma);
            ArgumentNullException.ThrowIfNull(dtxName);

            this.shapes = shapes;
            this.koma = koma;
            this.dtxName = dtxName;
        }

        /// <summary>
        /// Converts a <see cref="IBinary"/> (DTX4) to a <see cref="BinaryFormat"/> (PNG).
        /// </summary>
        /// <param name="source">DTX4 binary to convert.</param>
        /// <returns><see cref="BinaryFormat"/> containing the PNG.</returns>
        public BinaryFormat Convert(IBinary source)
        {
            ArgumentNullException.ThrowIfNull(source);

            source.Stream.Position = 0;

            // Parse the DTX4 into sprite + image
            var dtx4Converter = new BinaryDtx4ToSpriteImage();
            using NodeContainerFormat dtx4 = dtx4Converter.Convert(source);

            IndexedPaletteImage image = dtx4.Root.Children["image"]!.GetFormatAs<IndexedPaletteImage>()!;

            // We ignore the sprite info from the DSTX and we take the one from the kshape
            KomaElement komaElement = koma.First(n => n.KomaName == dtxName)
                ?? throw new FormatException($"Can't find '{dtxName}' in the koma.bin");

            Sprite sprite = shapes.GetSprite(komaElement.KShapeGroupId, komaElement.KShapeElementId);

            var spriteParams = new Sprite2IndexedImageParams {
                RelativeCoordinates = SpriteRelativeCoordinatesKind.TopLeft,
                FullImage = image,
            };

            return new Node("sprite", sprite)
                .TransformWith(new Sprite2IndexedImage(spriteParams))
                .TransformWith(new IndexedImage2BinaryPng(image!))
                .GetFormatAs<BinaryFormat>()!;
        }
    }
}
