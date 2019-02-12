//
// ColorQuantization.cs
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
namespace Texim.Media.Image.Processing
{
    using System.Drawing;

    public abstract class ColorQuantization
    {
        protected ColorQuantization()
        {
            TileSize = new Size(8, 8);
        }

        public int Width {
            get;
            private set;
        }

        public int Height {
            get;
            private set;
        }

        public Color[] Palette {
            get;
            protected set;
        }

        public Size TileSize {
            get;
            set;
        }

        protected Pixel[] Pixels {
            get;
            private set;
        }


        public void Quantizate(Bitmap image)
        {
            Width  = image.Width;
            Height = image.Height;

            Pixels = new Pixel[Width * Height];
            PreQuantization(image);

            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    Pixel px = QuantizatePixel(x, y);

                    int idx = y * Width + x;
                    Pixels[idx] = px;
                }
            }

            PostQuantization();
        }

        public Pixel[] GetPixels(PixelEncoding enc)
        {
            Pixel[] encoded = new Pixel[Width * Height];
            enc.Codec(Pixels, encoded, false, Width, Height, TileSize);
            return encoded;
        }

        protected abstract void PreQuantization(Bitmap image);

        protected abstract Pixel QuantizatePixel(int x, int y);

        protected abstract void PostQuantization();
    }
}

