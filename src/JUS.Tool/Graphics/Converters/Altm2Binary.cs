// Copyright (c) 2022 Priverop
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
using Texim.Games.Nitro.TileMaps;
using Texim.TileMaps;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUS.Tool.Graphics.Converters
{
    /// <summary>
    /// Converts between BinaryFormat and ALTM.
    /// </summary>
    public class Altm2Binary :
        IConverter<Altm, BinaryFormat>
    {
        /// <summary>
        /// Converts an Altm Node to a BinaryFormat Node.
        /// </summary>
        /// <param name="source">Altm Node.</param>
        /// <returns>BinaryFormat Node.</returns>
        public BinaryFormat Convert(Altm source)
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
