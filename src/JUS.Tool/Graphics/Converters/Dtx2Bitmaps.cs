// Copyright (c) 2025 Priverop

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
using JUS.Tool.Graphics.Converters;
using Texim.Formats;
using Texim.Sprites;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Graphics.Converters
{
    /// <summary>
    /// Converts between a IBinary (file with the dtx) and a NCF of Bitmaps (PNG).
    /// </summary>
    public class Dtx2Bitmaps :
        IConverter<IBinary, NodeContainerFormat>
    {
        /// <summary>
        /// Converts a <see cref="IBinary"/> (dtx) to a <see cref="NodeContainerFormat"/> containing all the PNGs (Sprites).
        /// </summary>
        /// <param name="source">File to convert.</param>
        /// <returns><see cref="NodeContainerFormat"/>.</returns>
        public NodeContainerFormat Convert(IBinary source)
        {
            ArgumentNullException.ThrowIfNull(source);

            source.Stream.Position = 0; // Just in case

            var converter = new BinaryToDtx3();

            // Sprites + pixels + palette
            using NodeContainerFormat dtx3 = converter.Convert(source);

            Dig image = dtx3.Root.Children["image"].GetFormatAs<Dig>();
            var spriteParams = new Sprite2IndexedImageParams {
                RelativeCoordinates = SpriteRelativeCoordinatesKind.Center,
                FullImage = image,
            };
            var indexedImageParams = new IndexedImageBitmapParams {
                Palettes = image,
            };

            var bitmaps = new NodeContainerFormat();

            switch (image.Swizzling) {
                case DigSwizzling.Tiled:
                    foreach (Node nodeSprite in dtx3.Root.Children["sprites"].Children) {
                        // Cloning the node so we can transform it
                        bitmaps.Root.Add(new Node(nodeSprite.Name, nodeSprite.GetFormatAs<Sprite>())
                            .TransformWith(new Sprite2IndexedImage(spriteParams))
                            .TransformWith(new IndexedImage2Bitmap(indexedImageParams)));
                    }

                    break;
                case DigSwizzling.Linear:
                    foreach (Node nodeTexture in dtx3.Root.Children["sprites"].Children) {
                        // Cloning the node so we can transform it
                        bitmaps.Root.Add(new Node(nodeTexture.Name, nodeTexture.GetFormatAs<Sprite>())
                            .TransformWith(new IndexedImage2Bitmap(indexedImageParams)));
                    }

                    break;
                default:
                    throw new FormatException("Invalid swizzling");
            }

            return bitmaps;
        }
    }
}
