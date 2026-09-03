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
    public class Binary2Altm :
        IConverter<IBinary, Altm>
    {
        /// <summary>
        /// Converts a IBinary Node to an Altm Node.
        /// </summary>
        /// <param name="source">IBinary Node.</param>
        /// <returns>Altm Node.</returns>
        public Altm Convert(IBinary source)
        {
            var reader = new DataReader(source.Stream);

            var altm = new Altm {
                Magic = reader.ReadUInt32(),
                Unknown = reader.ReadUInt32(),
                Unknown2 = reader.ReadUInt32(),
                TileSizeW = reader.ReadUInt16(),
                TileSizeH = reader.ReadUInt16(),
                NumTileW = reader.ReadUInt16(),
                NumTileH = reader.ReadUInt16(),
                Unknown3 = reader.ReadUInt32(),
            };

            altm.TileSize = new System.Drawing.Size(altm.TileSizeW, altm.TileSizeH);

            altm.Width = altm.TileSizeW * altm.NumTileW;
            altm.Height = (altm.TileSizeH * altm.NumTileH) + 8;

            altm.BgMode = NitroBackgroundMode.Text;

            long mapInfoSize = reader.Stream.Length - reader.Stream.Position;
            uint numInfos = (uint)((altm.BgMode == NitroBackgroundMode.Affine) ? mapInfoSize : mapInfoSize / 2);

            altm.Maps = new MapInfo[numInfos];
            for (int i = 0; i < altm.Maps.Length; i++) {
                altm.Maps[i] = altm.BgMode == NitroBackgroundMode.Affine ? new MapInfo(reader.ReadByte()) : MapInfo.FromUInt16(reader.ReadUInt16());
            }

            return altm;
        }
    }
}
