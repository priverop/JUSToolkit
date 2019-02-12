//
// PixelEncoding.cs
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

    public enum PixelEncoding {
        HorizontalTiles = 0,
        Lineal = 1,
        VerticalTiles,
        Unknown,
    }

    public static class PixelEncodingExtension
    {
        public static bool IsTiled(this PixelEncoding enc)
        {
            switch (enc) {
            case PixelEncoding.HorizontalTiles:
            case PixelEncoding.VerticalTiles:
                return true;

            case PixelEncoding.Lineal:
                return false;

            default:
                throw new FormatException();
            }
        }

        public static void Codec<T>(this PixelEncoding pxEnc, T[] dataIn, T[] dataOut, bool decoding,
                                       int width, int height, Size tileSize)
        {
            if (pxEnc != PixelEncoding.Lineal && pxEnc != PixelEncoding.HorizontalTiles && 
                pxEnc != PixelEncoding.VerticalTiles)
                throw new NotSupportedException();

            if (dataIn == null || dataOut == null || dataIn.Length > dataOut.Length)
                throw new ArgumentNullException();

            if ((width % tileSize.Width != 0) && 
                (pxEnc == PixelEncoding.HorizontalTiles || pxEnc == PixelEncoding.VerticalTiles))
                throw new FormatException("Width must be a multiple of tile width to use Tiled pixel encoding.");

            // Little trick to use the same equations
            if (pxEnc == PixelEncoding.Lineal)
                tileSize = new Size(width, height);

            for (int linealIndex = 0; linealIndex < dataOut.Length; linealIndex++) {
                int tiledIndex = pxEnc.GetIndex(linealIndex % width, linealIndex / width, width, height, tileSize);

                if (decoding) {
                    // As the new data is lineal, and in the last row of tiles in the dataIn can be incompleted
                    // the output array can contains null pixels in the middle of the array.
                    if (tiledIndex >= dataIn.Length)
                        dataOut[linealIndex] = default(T);    // Null pixel
                    else
                        dataOut[linealIndex] = dataIn[tiledIndex];
                } else {
                    // As this index will increment lineally, we can stop, there isn't more data to code
                    if (linealIndex >= dataIn.Length)
                        break;
                    dataOut[tiledIndex] = dataIn[linealIndex];
                }
            }
        }

        public static int GetIndex(this PixelEncoding pxEnc, int x, int y, int width, int height, Size tileSize)
        {
            if (pxEnc == PixelEncoding.Lineal)
                return y * width + x;

            int tileLength = tileSize.Width * tileSize.Height;
            int numTilesX = width / tileSize.Width;
            int numTilesY = height / tileSize.Height;

            // Get lineal index
            Point pixelPos = new Point(x % tileSize.Width, y % tileSize.Height); // Pos. pixel in tile
            Point tilePos  = new Point(x / tileSize.Width, y / tileSize.Height); // Pos. tile in image
            int index = 0;

            if (pxEnc == PixelEncoding.HorizontalTiles)
                index = tilePos.Y * numTilesX * tileLength + tilePos.X * tileLength;    // Absolute tile pos.
            else if (pxEnc == PixelEncoding.VerticalTiles)
                index = tilePos.X * numTilesY * tileLength + tilePos.Y * tileLength;    // Absolute tile pos.

            index += pixelPos.Y * tileSize.Width + pixelPos.X;    // Add pos. of pixel inside tile

            return index;
        }
    }
}

