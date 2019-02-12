//
// Image.cs
//
// Author:
//       Benito Palacios Sanchez <benito356@gmail.com>
//
// Copyright (c) 2017 Benito Palacios Sanchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
namespace Texim.Media.Image
{
    using System;
    using System.Drawing;
    using Yarhl.FileFormat;

    public class PixelArray : Format
    {
        // Image data will be independent of the value of "format" and "pixelEnc" doing a conversion to lineal pixel
        // encoding and to 24BPP index + 8 bits of alpha component if the image is indexed and to ABGR32 otherwise.
        // Doing so, operations and transformations will be easier to implement since there will be only two formats to
        // work. The conversion will take place at the initialization and when the data is required.
        // CHECK: What about changing the type to Pixel?
        uint[] data;
        uint[] original;    // Original data, without decoding it

        public PixelArray()
        {
            Format   = ColorFormat.Unknown;
            PixelEncoding = PixelEncoding.Unknown;
            TileSize = new Size(8, 8);
        }

        public PixelArray(Pixel[] pixels, int width, int height, PixelEncoding pxEnc, ColorFormat format, Size tileSize)
        {
            Width = width;
            Height = height;
            SetData(pixels, pxEnc, format, tileSize);
        }

        PixelArray(PixelArray img, uint[] data, int width, int height)
        {
            Width  = width;
            Height = height;
            original = data;
            Format = img.Format;
            PixelEncoding = img.PixelEncoding;
            TileSize = img.TileSize;

            data = new uint[data.Length];
            PixelEncoding.Codec(original, data, true, width, height, TileSize); 
        }

        public int Width {
            get;
            set;
        }

        public int Height {
            get;
            set;
        }

        public ColorFormat Format {
            get;
            set;
        }

        public PixelEncoding PixelEncoding {
            get;
            private set;
        }

        public Size TileSize {
            get;
            private set;
        }

        public Bitmap CreateBitmap()
        {
            if (Format.IsIndexed())
                throw new ArgumentException("A palette is required.");

            Bitmap bmp = new Bitmap(Width, Height);
            for (int i = 0; i < data.Length; i++)
                bmp.SetPixel(i % Width, i / Width, data[i].ToArgbColor());

            return bmp;
        }

        public Bitmap CreateBitmap(Palette palette, int paletteIndex)
        {
            if (!Format.IsIndexed()) {
                Console.WriteLine("##WARNING## The palette is not required.");
                return CreateBitmap();
            }

            Bitmap bmp = new Bitmap(Width, Height);
            var colors = InfoToIndexedColors(data, palette.GetPalette(paletteIndex));
            for (int i = 0; i < colors.Length; i++)
                bmp.SetPixel(i % Width, i / Width, colors[i]);

            return bmp;
        }

        public Pixel[] GetTile(int index)
        {
            int tileLength = TileSize.Width * TileSize.Height;
            Pixel[] tile = new Pixel[tileLength];

            bool isIndexed = Format.IsIndexed();

            for (int y = 0; y < TileSize.Height; y++) {
                for (int x = 0; x < TileSize.Width; x++) {
                    uint px = original[y * TileSize.Width + x + index * tileLength];
                    tile[y * TileSize.Width + x] = new Pixel(
                        px & 0x00FFFFFF,
                        (px >> 24) & 0xFF,
                        isIndexed);
                }
            }

            return tile;
        }

        public void SetData(byte[] raw, PixelEncoding pixelEnc, ColorFormat format)
        {
            SetData(raw, pixelEnc, format, new Size(8, 8));
        }

        public void SetData(byte[] raw, PixelEncoding pixelEnc, ColorFormat format, Size tileSize)
        {
            if (Width == 0 || Height == 0)
                throw new ArgumentException("Width and Height have not been specified.");

            PixelEncoding = pixelEnc;
            Format   = format;
            TileSize = tileSize;

            // First convert to 24bpp index + 8 bits alpha if it's indexed or ARGB32 otherwise.
            // normalizeData contains information about 1 pixel (index or color)
            original = new uint[raw.Length * 8 / Format.Bpp()];

            int rawPos = 0;
            for (int i = 0; i < original.Length; i++) {
                uint info = GetBits(raw, ref rawPos, Format.Bpp()); // Get pixel info from raw data
                original[i] = Format.UnpackColor(info);            // Get color from pixel info (unpack info)
            }

            // Then convert to lineal pixel encoding
            data = new uint[Width * Height];
            PixelEncoding.Codec(original, data, true, Width, Height, TileSize);
        }

        public void SetData(Pixel[] pixels, PixelEncoding pixelEnc, ColorFormat format, Size tileSize)
        {
            if (Width == 0 || Height == 0)
                throw new ArgumentException("Width and Height have not been specified.");

            PixelEncoding = pixelEnc;
            Format   = format;
            TileSize = tileSize;

            original = new uint[pixels.Length];
            for (int i = 0; i < pixels.Length; i++)
                original[i] = (uint)(pixels[i].Alpha << 24) | pixels[i].Info;

            data = new uint[Width * Height];
            PixelEncoding.Codec(original, data, true, Width, Height, TileSize);
        }

        public byte[] GetData()
        {
            // Inverse operation of SetData

            // Then code normalized data to its format and write to final buffer
            byte[] buffer = new byte[original.Length * Format.Bpp() / 8];
            int bufferPos = 0;

            for (int i = 0; i < original.Length; i++) {
                uint info = Format.PackColor(original[i]);
                SetBits(buffer, ref bufferPos, Format.Bpp(), info);
            }

            return buffer;
        }

        public Pixel[] GetPixels()
        {
            // Inverse operation of SetData

            // Then get the array of pixels
            Pixel[] pixels = new Pixel[original.Length];
            for (int i = 0; i < original.Length; i++)
                pixels[i] = new Pixel(
                    original[i] & 0xFFFFFF,
                    original[i] >> 24,
                    Format.IsIndexed()
                );

            return pixels;
        }

        static Color[] InfoToIndexedColors(uint[] colorInfo, Color[] palette)
        {
            return InfoToIndexedColors(
                colorInfo,
                new Color[][] { palette },
                new uint[colorInfo.Length]    // By default is filled with 0
            );
        }

        static Color[] InfoToIndexedColors(uint[] colorInfo, Color[][] palettes, uint[] palIdx)
        {
            Color[] colors = new Color[colorInfo.Length];
            for (int i = 0; i < colorInfo.Length; i++)
                colors[i] = InfoToIndexedColor(colorInfo[i], palettes[palIdx[i]]);
            return colors;
        }

        static Color InfoToIndexedColor(uint colorInfo, Color[] palette)
        {
            uint alpha      = colorInfo >> 24;
            uint colorIndex = colorInfo & 0x00FFFFFF;
            if (colorIndex >= palette.Length)
                throw new IndexOutOfRangeException("Color index out of palette");

            Color color = palette[colorIndex];
            color = Color.FromArgb((int)alpha, color);

            return color;
        }

        static uint GetBits(byte[] data, ref int bitPos, int size)
        {
            if (size < 0 || size > 32)
                throw new ArgumentOutOfRangeException(nameof(size));

            if (bitPos + size > data.Length * 8)
                throw new IndexOutOfRangeException();

            uint value = 0;
            for (int s = 0; s < size; s++, bitPos++) {
                uint bit = data[bitPos / 8];
                bit >>= (bitPos % 8);
                bit &= 1;

                value |= bit << s;
            }

            return value;
        }

        static void SetBits(byte[] data, ref int bitPos, int size, uint value)
        {
            if (size < 0 || size > 32)
                throw new ArgumentOutOfRangeException(nameof(size));

            if (bitPos + size > data.Length * 8)
                throw new IndexOutOfRangeException();

            for (int s = 0; s < size; s++, bitPos++) {
                uint bit = (value >> s) & 1;

                uint val = data[bitPos / 8];
                val |= bit << (bitPos % 8);
                data[bitPos / 8] = (byte)val;
            }
        }
    }
}
