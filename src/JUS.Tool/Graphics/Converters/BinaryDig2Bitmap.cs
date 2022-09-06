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
using Texim;
using Texim.Colors;
using Texim.Palettes;
using Texim.Pixels;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Graphics.Converters
{
    /// <summary>
    /// Converts between BinaryFormat (a file) containing a Dsig Format and IndexedPaletteImage (PNG).
    /// </summary>
    public class BinaryDig2Bitmap : IConverter<IBinary, BinaryFormat>
    {
        /// <summary>
        /// Converts a <see cref="IBinary"/> (file) to a <see cref="BinaryFormat"/>.
        /// </summary>
        /// <param name="source">File to convert.</param>
        /// <returns><see cref="Dig"/>.</returns>
        public BinaryFormat Convert(IBinary source)
        {
            if (source is null) {
                throw new ArgumentNullException(nameof(source));
            }

            using var pixelsPaletteNode = NodeFactory.FromFile(dig, FileOpenMode.Read)
                .TransformWith<LzssDecompression>()
                .TransformWith<Binary2Dig>();

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

            return dig;
        }
    }
}
