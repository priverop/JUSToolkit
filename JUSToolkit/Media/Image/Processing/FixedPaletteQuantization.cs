//
// FixedPaletteQuantization.cs
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

    public class FixedPaletteQuantization : ColorQuantization
    {
        NearestNeighbour<Color> nearestNeighbour;
        Bitmap image;

        public FixedPaletteQuantization(Color[] fixedPalette)
        {
            nearestNeighbour = new ExhaustivePaletteSearch();
            Palette = fixedPalette;
        }

        public uint TransparentIndex {
            get;
            set;
        }

        protected override void PreQuantization(Bitmap image)
        {
            this.image = image;
            nearestNeighbour.Initialize(Palette);
        }

        protected override Pixel QuantizatePixel(int x, int y)
        {
            Color imgColor = image.GetPixel(x, y);

            // If it's a transparent color, set the first palette color
            if (imgColor.A == 0)
                return new Pixel(TransparentIndex, 0, true);

            // Get nearest color from palette
            int colorIndex = nearestNeighbour.Search(imgColor);
            return new Pixel((uint)colorIndex, imgColor.A, true);
        }

        protected override void PostQuantization()
        {
        }
    }
}
