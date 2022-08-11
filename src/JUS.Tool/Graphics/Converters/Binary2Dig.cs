// Copyright (c) 2021 SceneGate

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
        /// Converts a BinaryFormat (file) to a Dig.
        /// </summary>
        /// <param name="source">File to convert.</param>
        /// <returns>Dig.</returns>
        public Dig Convert(IBinary source)
        {
            if (source is null) {
                throw new ArgumentNullException(nameof(source));
            }

            var reader = new DataReader(source.Stream);
            var dig = new Dig();
            source.Stream.Position = 0;

            if (reader.ReadString(4) != Dig.STAMP) {
                throw new FormatException("Invalid stamp");
            }

            dig.Unknown = reader.ReadByte();
            dig.ImageFormat = reader.ReadByte();
            bool is8Bpp = dig.ImageFormat != 0x10;
            dig.NumPalettes = reader.ReadUInt16();
            dig.Width = reader.ReadUInt16();
            dig.Height = reader.ReadUInt16();

            int colorsPerPalette = is8Bpp ? 256 : 16;

            // 8Bpp dig files only have one 256 colors palette
            for (int i = 0; i < (is8Bpp ? 1 : dig.NumPalettes); i++) {
                dig.PaletteCollection.Palettes.Add(new Palette(reader.ReadColors<Bgr555>(colorsPerPalette)));
            }

            IIndexedPixelEncoding pixelEncoding = is8Bpp ? Indexed8Bpp.Instance : Indexed4Bpp.Instance;
            dig.Pixels = pixelEncoding.Decode(source.Stream, dig.Width * dig.Height);

            return dig;
        }
    }
}
