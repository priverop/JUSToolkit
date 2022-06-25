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
namespace Texim.Games.JumpUltimateStars
{
    using System;
    using Texim.Colors;
    using Texim.Images;
    using Texim.Palettes;
    using Texim.Pixels;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    public class BinaryDsig2IndexedPaletteImage : IConverter<IBinary, IndexedPaletteImage>
    {
        public IndexedPaletteImage Convert(IBinary source)
        {
            if (source is null) {
                throw new ArgumentNullException(nameof(source));
            }

            var reader = new DataReader(source.Stream);
            source.Stream.Position = 0;

            if (reader.ReadString(4) != "DSIG") {
                throw new FormatException("Invalid stamp");
            }

            reader.ReadByte();
            bool is8Bpp = reader.ReadByte() != 0x10;
            short numPalettes = reader.ReadInt16();
            int width = reader.ReadUInt16();
            int height = reader.ReadUInt16();

            var palettes = new PaletteCollection();
            int colorsPerPalette = is8Bpp ? 256 : 16;
            for (int i = 0; i < numPalettes; i++) {
                palettes.Palettes.Add(new Palette(reader.ReadColors<Bgr555>(colorsPerPalette)));
            }

            IIndexedPixelEncoding pixelEncoding = is8Bpp ? Indexed8Bpp.Instance : Indexed4Bpp.Instance;
            var pixels = pixelEncoding.Decode(source.Stream, width * height)
                .UnswizzleWith(new TileSwizzling<IndexedPixel>(width));

            var image = new IndexedPaletteImage {
                Width = width,
                Height = height,
                Pixels = pixels,
            };
            image.Palettes.Add(palettes.Palettes);

            return image;
        }
    }
}
