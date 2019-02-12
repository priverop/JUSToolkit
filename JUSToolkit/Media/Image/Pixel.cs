//
// Pixel.cs
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
    using System.Drawing;

    public struct Pixel
    {
        public Pixel(uint info, uint alpha, bool isIndexed) : this()
        {
            IsIndexed = isIndexed;
            Info = info;
            Alpha = (byte)alpha;
        }

        public bool IsIndexed {
            get;
            private set;
        }

        /// <summary>
        /// Gets the pixel info.
        /// If it's indexed it returns the color index otherwise, it returns a 32bit BGR value.
        /// </summary>
        /// <value>The pixel info.</value>
        public uint Info {
            get;
            private set;
        }

        public byte Alpha {
            get;
            private set;
        }

        public Pixel ChangeInfo(uint info)
        {
            return new Pixel(info, Alpha, IsIndexed);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(Pixel))
                return false;
            Pixel other = (Pixel)obj;
            return IsIndexed == other.IsIndexed && Info == other.Info && Alpha == other.Alpha;
        }

        public override int GetHashCode()
        {
            unchecked {
                return IsIndexed.GetHashCode() ^ Info.GetHashCode() ^ Alpha.GetHashCode();
            }
        }
            
        public override string ToString()
        {
            return string.Format("[Pixel: IsIndexed={0}, Info={1}, Alpha={2}]", IsIndexed, Info, Alpha);
        }
    }

    public static class PixelExtension
    {
        public static void FlipX(this Pixel[] tile, Size tileSize)
        {
            for (int y = 0; y < tileSize.Height; y++) {
                for (int x = 0; x < tileSize.Width / 2; x++) {
                    int t1 = y * tileSize.Width + x;
                    int t2 = y * tileSize.Width + (tileSize.Width - 1 - x);

                    Pixel swap = tile[t1];
                    tile[t1] = tile[t2];
                    tile[t2] = swap;
                }
            }
        }

        public static void FlipY(this Pixel[] tile, Size tileSize)
        {
            for (int x = 0; x < tileSize.Width; x++) {
                for (int y = 0; y < tileSize.Height / 2; y++) {
                    int t1 = x + tileSize.Width * y;
                    int t2 = x + tileSize.Width * (tileSize.Height - 1 - y);

                    Pixel swap = tile[t1];
                    tile[t1] = tile[t2];
                    tile[t2] = swap;
                }
            }
        }
    }
}

