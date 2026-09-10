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
using JUS.Tool.Utils;
using Texim.Images;
using Texim.Formats.ImageSharp.Images;
using Texim.TileMaps;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Graphics.Converters
{
    /// <summary>
    /// Converts between BinaryFormat (a file) containing a Dsig Format and IndexedPaletteImage (PNG).
    /// </summary>
    public class BinaryDig2Bitmap :
        IConverter<IBinary, BinaryFormat>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryDig2Bitmap"/> class.
        /// </summary>
        /// <param name="atm">Node with the required map.</param>
        public BinaryDig2Bitmap(Node atm)
        {
            OriginalAtm = atm;
        }

        /// <summary>
        /// Gets or sets the Original Atm.
        /// </summary>
        public Node OriginalAtm { get; set; }

        /// <summary>
        /// Converts a <see cref="Node"/> (file) to a <see cref="BinaryFormat"/>.
        /// </summary>
        /// <param name="source">File to convert.</param>
        /// <returns><see cref="Dig"/>.</returns>
        public BinaryFormat Convert(IBinary source)
        {
            if (source is null) {
                throw new ArgumentNullException(nameof(source));
            }

            var decompression = new LzssDecompression();

            BinaryFormat uncompressedDig = decompression.Convert(source);
            Dig pixelsPalette = new Binary2Dig().Convert(uncompressedDig);

            // Map
            BinaryFormat uncompressedAtm = decompression.Convert(OriginalAtm.GetFormatAs<IBinary>());
            Altm maps = new Binary2Altm().Convert(uncompressedAtm);

            var mapsParams = new MapDecompressionParams {
                Map = maps,
                TileSize = maps.TileSize,
            };

            IIndexedImage image = new MapDecompression(mapsParams).Convert(pixelsPalette);

            return new IndexedImage2BinaryPng(pixelsPalette).Convert(image);
        }
    }
}
