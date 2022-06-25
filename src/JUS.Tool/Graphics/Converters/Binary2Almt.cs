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
using JUSToolkit.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Converters.Images
{
    /// <summary>
    /// Converts between BinaryFormat and ALMT.
    /// </summary>
    public class Binary2Almt :
        IConverter<BinaryFormat, Almt>,
        IConverter<Almt, BinaryFormat>
    {
        /// <summary>
        /// Converts a BinaryFormat Node to an Almt Node.
        /// </summary>
        /// <param name="source">BinaryFormat Node.</param>
        /// <returns>Almt Node.</returns>
        public Almt Convert(BinaryFormat source)
        {
            var almt = new Almt();

            var reader = new DataReader(source.Stream);

            almt.Magic = reader.ReadUInt32();

            almt.Unknown = reader.ReadUInt32();
            almt.Unknown2 = reader.ReadUInt32();

            almt.TileSizeW = reader.ReadUInt16();
            almt.TileSizeH = reader.ReadUInt16();

            almt.NumTileW = reader.ReadUInt16();
            almt.NumTileH = reader.ReadUInt16();

            almt.Unknown3 = reader.ReadUInt32();

            almt.TileSize = new System.Drawing.Size(almt.TileSizeW, almt.TileSizeH);

            almt.Width = (int)(almt.TileSizeW * almt.NumTileW);
            almt.Height = (int)(almt.TileSizeH * almt.NumTileH) + 8;

            almt.BgMode = BgMode.Text;

            long mapInfoSize = reader.Stream.Length - reader.Stream.Position;
            uint numInfos = (uint)((almt.BgMode == BgMode.Affine) ? mapInfoSize : mapInfoSize / 2);

            almt.Maps = new MapInfo[numInfos];
            for (int i = 0; i < almt.Maps.Length; i++) {
                if (almt.BgMode == BgMode.Affine) {
                    almt.Maps[i] = new MapInfo(reader.ReadByte());
                } else {
                    almt.Maps[i] = new MapInfo(reader.ReadUInt16());
                }
            }

            almt.SetMapInfo(almt.Maps);

            return almt;
        }

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
            foreach (Texim.Compressions.Nitro.MapInfo info in source.Maps) {
                if (source.BgMode == BgMode.Affine) {
                    writer.Write(info.ToByte());
                } else {
                    writer.Write(info.ToUInt16());
                }
            }

            return b;
        }
    }
}
