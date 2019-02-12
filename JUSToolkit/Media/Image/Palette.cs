//
// Palette.cs
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
    using System.Collections.Generic;
    using System.IO;
    using System.Drawing;
    using Yarhl.FileFormat;

    public class Palette : Format
    {
        Color[][] palette;

        public Palette()
        {
            palette = new Color[0][];
        }

        public Palette(Color[] palette)
        {
            SetPalette(palette);
        }

        public Palette(Color[][] palette)
        {
            SetPalette(palette);
        }

        public int NumPalettes {
            get { return palette.Length; }
        }

        public IEnumerable<Color[]> Palettes {
            get {
                foreach (Color[] pal in this.palette) {
                    yield return (Color[])pal.Clone();
                }
            }
        }

        public Color GetColor(int paletteIndex, int colorIndex)
        {
            if (paletteIndex < 0 || paletteIndex >= palette.Length)
                throw new IndexOutOfRangeException();

            if (colorIndex < 0 || colorIndex >= palette[paletteIndex].Length)
                throw new IndexOutOfRangeException();

            return palette[paletteIndex][colorIndex];
        }

        public Color[] GetPalette(int index)
        {
            if (index < 0 || index >= palette.Length)
                throw new IndexOutOfRangeException();

            return (Color[])palette[index].Clone();
        }

        public Color[][] GetPalettes()
        {
            return (Color[][])palette.Clone();
        }

        public void SetPalette(Color[] palette)
        {
            this.palette = new Color[1][];
            this.palette[0] = new Color[palette.Length];
            Array.Copy(palette, this.palette[0], palette.Length);
        }

        public void SetPalette(Color[][] palette)
        {
            this.palette = new Color[palette.Length][];
            for (int i = 0; i < palette.Length; i++) {
                this.palette[i] = new Color[palette[i].Length];
                Array.Copy(palette[i], this.palette[i], palette[i].Length);
            }
        }

        public Bitmap CreateBitmap(int index)
        {
            if (index < 0 || index >= this.palette.Length)
                throw new IndexOutOfRangeException();

            return CreateBitmap(palette[index]);
        }

        public static Bitmap CreateBitmap(Color[] colors)
        {
            int height = (colors.Length / 0x10);
            if (colors.Length % 0x10 != 0)
                height++;

            Bitmap palette = new Bitmap(160, height * 10);

            bool end = false;
            for (int i = 0; i < 16 & !end; i++) {
                for (int j = 0; j < 16; j++) {
                    // Check if we have reached the end.
                    // A palette image can be incompleted (number of colors not padded to 16)
                    if (colors.Length <= j + 16 * i) {
                        end = true;
                        break;
                    }

                    for (int k = 0; k < 10; k++)
                        for (int q = 0; q < 10; q++)
                            palette.SetPixel(j * 10 + q, i * 10 + k, colors[j + 16 * i]);
                }
            }

            return palette;
        }

        public void ToWinPaletteFormat(string outPath, int index, bool gimpCompability)
        {
            FileStream stream = new FileStream(outPath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(stream);

            bw.Write(new char[] { 'R', 'I', 'F', 'F' });        // "RIFF"
            bw.Write((uint)(0x10 + palette[index].Length * 4)); // file_length - 8
            bw.Write(new char[] { 'P', 'A', 'L', ' ' });        // "PAL "
            bw.Write(new char[] { 'd', 'a', 't', 'a' });        // "data"
            bw.Write((uint)palette[index].Length * 4 + 4);      // data_size = file_length - 0x14
            bw.Write((ushort)0x0300);                           // version = 00 03
            bw.Write((ushort)(palette[index].Length));          // num_colors
            if (gimpCompability)
                bw.Write((uint)0x00);                           // Error in Gimp 2.8

            for (int i = 0; i < palette[index].Length; i++)
            {
                bw.Write(palette[index][i].R);
                bw.Write(palette[index][i].G);
                bw.Write(palette[index][i].B);
                bw.Write((byte)0x00);
                bw.Flush();
            }

            stream.Close();
        }

        public void ToAcoFormat(string outPath, int index)
        {
            FileStream stream = new FileStream(outPath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(stream);

            bw.Write((ushort)0x00);                             // Version 0
            bw.Write(ToBytesBE((ushort)palette[index].Length)); // Number of colors

            for (int i = 0; i < palette[index].Length; i++) {
                bw.Write((ushort)0x00);                    // Color spec set to 0
                bw.Write(ToBytesBE(palette[index][i].R));  // Red component
                bw.Write(ToBytesBE(palette[index][i].G));  // Green component
                bw.Write(ToBytesBE(palette[index][i].B));  // Blue component
                bw.Write((ushort)0x00);                    // Always 0x00, not used
            }

            stream.Close();
        }

        static byte[] ToBytesBE(ushort value)
        {
            byte[] data = BitConverter.GetBytes(value);
            return new byte[] { data[1], data[0] };
        }
    }
}
