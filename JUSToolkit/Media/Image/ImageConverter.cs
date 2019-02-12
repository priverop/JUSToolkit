//
// Importer.cs
//
// Author:
//       Benito Palacios Sanchez <benito356@gmail.com>
//
// Copyright (c) 2017 Benito Palacios Sanchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
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
    using Processing;

    public class ImageConverter : IConverter<Bitmap, Tuple<Palette, PixelArray>>
    {
        public ImageConverter()
        {
            // Default parameters: 
            // + Image will be HorizontalTiled
            // + Depth will be 8bpp (256/1).
            // + Transparent color will be magenta: (R:248, G:0, B:248) 
            Format        = ColorFormat.Indexed_8bpp;
            TileSize      = new Size(8, 8);
            PixelEncoding = PixelEncoding.HorizontalTiles;
            Quantization  = new BasicQuantization();
        }

        public ColorQuantization Quantization {
            get;
            set;
        }

        public ColorFormat Format {
            get;
            set;
        }

        public PixelEncoding PixelEncoding {
            get;
            set;
        }

        public Size TileSize {
            get;
            set;
        }

        public Tuple<Palette, PixelArray> Convert(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            int width  = bitmap.Width;
            int height = bitmap.Height;
            int maxColors = Format.MaxColors();

            // Quantizate image -> get pixels and palette
            Quantization.Quantizate(bitmap);
            Pixel[] pixels  = Quantization.GetPixels(this.PixelEncoding);
            Color[] colors = Quantization.Palette;
            if (colors.Length > maxColors)
                throw new FormatException($"The image has more than {maxColors} colors");

            // Create palette format
            Palette palette = new Palette(colors);

            // Create image format
            PixelArray image = new PixelArray();
            image.Width  = (pixels.Length > 256) ? 256 : pixels.Length;
            image.Height = (int)Math.Ceiling(pixels.Length / (double)image.Width);
            if (image.Height % TileSize.Height != 0)
                image.Height += TileSize.Height - (image.Height % TileSize.Height);
            image.SetData(pixels, PixelEncoding, Format, TileSize);

            return new Tuple<Palette, PixelArray>(palette, image);
        }
    }
}

