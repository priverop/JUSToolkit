using JUSToolkit.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Converters.Images
{
    public class Binary2Almt :
        IConverter<BinaryFormat, Almt>,
        IConverter<Almt, BinaryFormat>
    {
        public Almt Convert(BinaryFormat source)
        {
            var almt = new Almt();

            DataReader reader = new DataReader(source.Stream);

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
                if (almt.BgMode == BgMode.Affine)
                    almt.Maps[i] = new MapInfo(reader.ReadByte());
                else
                    almt.Maps[i] = new MapInfo(reader.ReadUInt16());
            }

            almt.SetMapInfo(almt.Maps);

            return almt;

        }

        public BinaryFormat Convert(Almt source)
        {
            var b = new BinaryFormat();

            DataWriter writer = new DataWriter(b.Stream);

            writer.Write(source.Magic);
            writer.Write(source.Unknown);
            writer.Write(source.Unknown2);
            writer.Write(source.TileSizeW);
            writer.Write(source.TileSizeH);
            writer.Write(source.NumTileW);
            writer.Write(source.NumTileH);
            writer.Write(source.Unknown3);
            foreach (MapInfo info in source.Maps) {
                if (source.BgMode == BgMode.Affine)
                    writer.Write(info.ToByte());
                else
                    writer.Write(info.ToUInt16());
            }

            return b;
        }
    }
}
