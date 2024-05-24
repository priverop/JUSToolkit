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
    public class Binary2Dig : IConverter<IBinary, Dig>
    {
        /// <summary>
        /// Converts a <see cref="BinaryFormat"/> (file) to a <see cref="Dig"/>.
        /// </summary>
        /// <param name="source">File to convert.</param>
        /// <returns><see cref="Dig"/>.</returns>
        public Dig Convert(IBinary source)
        {
            if (source is null) {
                throw new ArgumentNullException(nameof(source));
            }

            var reader = new DataReader(source.Stream);
            source.Stream.Position = 0;

            if (reader.ReadString(4) != Dig.STAMP) {
                throw new FormatException("Invalid stamp");
            }

            byte unknown = reader.ReadByte();
            byte imageFormat = reader.ReadByte();
            ushort numPaletteLines = reader.ReadUInt16();
            int width = reader.ReadUInt16();
            int height = reader.ReadUInt16();
            uint pixelsStart = (uint)((numPaletteLines * 0x20) + 0xC);
            var bpp = (DigBpp)(imageFormat & 0x0F);
            var swizzling = (DigSwizzling)(imageFormat >> 4);
            IIndexedPixelEncoding pixelEncoding;
            int colorsPerPalette;
            int numPalettes;

            switch (bpp) {
                case DigBpp.Bpp4:
                    pixelEncoding = Indexed4Bpp.Instance;
                    colorsPerPalette = 16;
                    numPalettes = numPaletteLines;
                    break;
                case DigBpp.Bpp8:
                    pixelEncoding = Indexed8Bpp.Instance;
                    colorsPerPalette = 256;
                    numPalettes = ((numPaletteLines - 1) / 16) + 1;
                    break;
                default:
                    throw new FormatException("Invalid bpp");
            }

            // Some tiled digs have fake size params
            if (swizzling == DigSwizzling.Tiled) {
                width = 8;
                height = bpp switch {
                    DigBpp.Bpp4 => (int)(source.Stream.Length - pixelsStart) / 4,
                    DigBpp.Bpp8 => (int)(source.Stream.Length - pixelsStart) / 8,
                    _ => throw new FormatException("Invalid bpp"),
                };
            }

            source.Stream.Position = pixelsStart;

            IndexedPixel[] pixels = swizzling switch {
                DigSwizzling.Tiled => pixelEncoding.Decode(source.Stream, width * height)
                        .UnswizzleWith(new TileSwizzling<IndexedPixel>(width)),
                DigSwizzling.Linear => pixelEncoding.Decode(source.Stream, width * height),
                _ => throw new FormatException("Invalid swizzling"),
            };

            var dig = new Dig {
                Unknown = unknown,
                ImageFormat = imageFormat,
                NumPaletteLines = numPaletteLines,
                Width = width,
                Height = height,
                Pixels = pixels,
                PixelsStart = pixelsStart,
                Bpp = bpp,
                Swizzling = swizzling,
            };

            for (int i = 0; i < numPalettes; i++) {
                dig.Palettes.Add(new Palette(reader.ReadColors<Bgr555>(colorsPerPalette)));
            }

            return dig;
        }
    }
}
