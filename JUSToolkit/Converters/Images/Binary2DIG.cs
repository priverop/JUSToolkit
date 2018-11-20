namespace JUSToolkit.Converters.Images
{
    using System;
    using Yarhl.FileFormat;
    using JUSToolkit.Formats;
    using Yarhl.IO;
    using Texim.Media.Image;
    using System.Drawing;
    using log4net;

    public class Binary2DIG : IConverter<BinaryFormat, DIG>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Identify));

        public DIG Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            DataReader reader = new DataReader(source.Stream);
            reader.Stream.Position = 0;

            DIG dig = new DIG
            {
                Magic = reader.ReadBytes(4),
                Type = reader.ReadByte(),
                Type_alt = reader.ReadByte(),
                PaletteNumber = reader.ReadByte(),
                Unknown = reader.ReadByte(),
                Unk = reader.ReadUInt16(),
                Unk2 = reader.ReadUInt16()
            };

            long startPalette = reader.Stream.Position;

            int returningBytes = 8;

            if(reader.ReadInt32() == 0){
                while (reader.ReadInt32() == 0) { }
                startPalette = reader.Stream.Position - 4;
                returningBytes += 4;
            }
            else{
                reader.Stream.Position = startPalette;
            }

            while (reader.ReadInt64() != 0) { }

            long endPalette = reader.Stream.Position - returningBytes;

            long paletteSize = endPalette - startPalette;

            reader.Stream.Position = startPalette;

            dig.Palette = new Palette(reader.ReadBytes((int)paletteSize).ToBgr555Colors());

            dig.Pixels = new PixelArray
            {
                Width = 256,
                Height = 192,
            };

            long bytesUntilEnd = reader.Stream.Length - reader.Stream.Position;

            ColorFormat format = paletteSize == 512 
                ? ColorFormat.Indexed_8bpp : ColorFormat.Indexed_4bpp;

            dig.Pixels.SetData(
                reader.ReadBytes((int)bytesUntilEnd),
                PixelEncoding.HorizontalTiles,
                format,
                new Size(8, 8));

            return dig;
        }
    }
}
