namespace JUSToolkit.Converters.Images
{
    using System;
    using JUSToolkit.Formats;
    using Texim;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    public class Binary2ALMT : IConverter<BinaryFormat, ALMT>
    {

        public ALMT Convert(BinaryFormat source)
        {
            var almt = new ALMT();

            DataReader reader = new DataReader(source.Stream);

            reader.Stream.Position = 0xC;

            almt.TileSizeW = reader.ReadUInt16();
            almt.TileSizeH = reader.ReadUInt16();

            almt.NumTileW = reader.ReadUInt16();
            almt.NumTileH = reader.ReadUInt16();

            almt.Unknown = reader.ReadUInt32();

            almt.TileSize = new System.Drawing.Size(almt.TileSizeW, almt.TileSizeH);

            almt.Width = (int) (almt.TileSizeW * almt.NumTileW);
            almt.Height = (int)(almt.TileSizeH * almt.NumTileH) + 8;

            almt.BgMode = BgMode.Text;

            long mapInfoSize = reader.Stream.Length - reader.Stream.Position;
            uint numInfos = (uint)((almt.BgMode == BgMode.Affine) ? mapInfoSize : mapInfoSize / 2);

            almt.Info = new MapInfo[numInfos];
            for (int i = 0; i < almt.Info.Length; i++)
            {
                if (almt.BgMode == BgMode.Affine)
                    almt.Info[i] = new MapInfo(reader.ReadByte());
                else
                    almt.Info[i] = new MapInfo(reader.ReadUInt16());
            }

            almt.SetMapInfo(almt.Info);

            return almt;

        }
    }
}
