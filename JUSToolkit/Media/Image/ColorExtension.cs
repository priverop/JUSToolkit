//
// ColorExtension.cs
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

    public static class ColorExtension
    {
        public static int CompareTo(this Color c1, Color c2)
        {
            if (c1.R == c2.R) {
                if (c1.G == c2.G) {
                    return c1.B.CompareTo(c2.B);
                } else {
                    return c1.G.CompareTo(c2.G);
                }
            } else {
                return c1.R.CompareTo(c2.R);
            }
        }

        public static double GetDistance(this Color c1, Color c2)
        {
            return Math.Sqrt(c1.GetDistanceSquared(c2));
        }

        public static double GetDistanceSquared(this Color c1, Color c2)
        {
            return (c2.R - c1.R) * (c2.R - c1.R) +
                (c2.G - c1.G) * (c2.G - c1.G) +
                (c2.B - c1.B) * (c2.B - c1.B);
        }

        public static Color[] ToArgbColors(this uint[] argb)
        {
            Color[] colors = new Color[argb.Length];
            for (int i = 0; i < argb.Length; i++)
                colors[i] = argb[i].ToArgbColor();
            return colors;
        }

        public static Color ToArgbColor(this uint argb)
        {
            return Color.FromArgb(
                (int)(argb >> 24) & 0xFF,
                (int)(argb >> 00) & 0xFF,
                (int)(argb >> 08) & 0xFF,
                (int)(argb >> 16) & 0xFF
            );
        }

        public static uint ToArgb(this Color color)
        {
            return (uint)(
                (color.R << 16) |
                (color.G << 08) |
                (color.B << 00) |
                (color.A << 24)
            );
        }

        public static ushort ToBgr555(this Color color)
        {
            int red   = color.R / 8;
            int green = color.G / 8;
            int blue  = color.B / 8;

            return (ushort)((red << 0) | (green << 5) | (blue << 10));
        }

        public static byte[] ToBgr555(this Color[] colors)
        {
            byte[] values = new byte[colors.Length * 2];

            for (int i = 0; i < colors.Length; i++) {
                ushort bgr = colors[i].ToBgr555();
                Array.Copy(BitConverter.GetBytes(bgr), 0, values, i * 2, 2);
            }

            return values;
        }

        public static Color ToBgr555Color(this ushort value)
        {
            int red   = ((value & 0x001F) >> 00) * 8;
            int green = ((value & 0x03E0) >> 05) * 8;
            int blue  = ((value & 0x7C00) >> 10) * 8;

            return Color.FromArgb(red, green, blue);
        }

        public static Color[] ToBgr555Colors(this byte[] values)
        {
            if (values.Length % 2 != 0)
                throw new ArgumentException("Length must be even.");

            Color[] colors = new Color[values.Length / 2];
            for (int i = 0; i < colors.Length; i++) {
                colors[i] = BitConverter.ToUInt16(values, i*2).ToBgr555Color();
            }

            return colors;
        }
    }
}

