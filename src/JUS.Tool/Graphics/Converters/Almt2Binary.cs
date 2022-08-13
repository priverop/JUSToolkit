// Copyright (c) 2022 Pablo Rivero
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using Texim.Compressions.Nitro;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Graphics.Converters
{
    /// <summary>
    /// Converts between BinaryFormat and ALMT.
    /// </summary>
    public class Almt2Binary :
        IConverter<Almt, BinaryFormat>
    {
        /// <summary>
        /// Converts an Almt Node to a BinaryFormat Node.
        /// </summary>
        /// <param name="source">Almt Node.</param>
        /// <returns>BinaryFormat Node.</returns>
        public BinaryFormat Convert(Almt source)
        {
            var b = new BinaryFormat();

            var writer = new DataWriter(b.Stream);

            writer.Write(source.Magic);
            writer.Write(source.Unknown);
            writer.Write(source.Unknown2);
            writer.Write(source.TileSizeW);
            writer.Write(source.TileSizeH);
            writer.Write(source.NumTileW);
            writer.Write(source.NumTileH);
            writer.Write(source.Unknown3);
            foreach (MapInfo info in source.Maps) {
                if (source.BgMode == NitroBackgroundMode.Affine) {
                    writer.Write(info.TileIndex);
                } else {
                    writer.Write(info.ToInt16());
                }
            }

            return b;
        }
    }
}
