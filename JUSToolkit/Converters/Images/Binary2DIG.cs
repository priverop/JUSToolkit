namespace JUSToolkit.Converters.Images
{
    using System;
    using Yarhl.FileFormat;
    using JUSToolkit.Formats;
    using Yarhl.IO;
    using Texim.Media.Image;
    using System.Drawing;
    using log4net;

    public class Binary2DIG : 
    IConverter<BinaryFormat, DIG>, 
    IConverter<DIG, BinaryFormat>
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
                PaletteType = reader.ReadByte(),
                PaletteSize = reader.ReadByte(),
                Unknown = reader.ReadByte(),
                Width = reader.ReadUInt16(),
                Height = reader.ReadUInt16()
            };

            long paletteEnd = dig.PaletteSize * 32 + reader.Stream.Position;
            log.Debug("Palette End: " + paletteEnd);

            long startPalette = reader.Stream.Position;

            int returningBytes = 8;

            // Si hay bytes vacios antes de empezar la paleta
            //*** Revisar
            if(reader.ReadInt32() == 0)
            {
                while (reader.ReadInt32() == 0) { }
                startPalette = reader.Stream.Position - 4;
                returningBytes += 4;
            }
            else{
                reader.Stream.Position = startPalette;
            }

            dig.PaletteStart = (uint) startPalette;
            log.Debug("Palette Start: " + dig.PaletteStart);

            long paletteActualSize = paletteEnd - dig.PaletteStart;
            log.Debug("Palette Size: " + paletteActualSize);

            reader.Stream.Position = dig.PaletteStart;

            ColorFormat format;

            if (dig.PaletteType == 16)
            {
                int paletteColors = 16;
                int paletteSize = paletteColors * 2;
                format = ColorFormat.Indexed_4bpp;
                Color[][] palettes;
                decimal paletteNumber = Math.Ceiling((decimal)paletteActualSize / paletteSize);
                log.Debug("Palette Number: " + paletteNumber);
                palettes = new Color[(int)paletteNumber][];

                for(int i = 0; i < paletteNumber; i++)
                {
                    palettes[i] = reader.ReadBytes((int)paletteSize).ToBgr555Colors();
                }

                dig.Palette = new Palette(palettes);
            }
            else
            {
                format = ColorFormat.Indexed_8bpp;
                dig.Palette = new Palette(reader.ReadBytes((int)paletteActualSize).ToBgr555Colors());

            }

            dig.Pixels = new PixelArray
            {
                Width = dig.Width,
                Height = dig.Height,
            };

            long bytesUntilEnd = reader.Stream.Length - reader.Stream.Position;

            dig.Pixels.SetData(
                reader.ReadBytes((int)bytesUntilEnd),
                PixelEncoding.HorizontalTiles,
                format,
                new Size(8, 8));

            return dig;
        }

        public BinaryFormat Convert(DIG dig)
        {
            if (dig == null)
                throw new ArgumentNullException(nameof(dig));

            var binary = new BinaryFormat();

            DataWriter writer = new DataWriter(binary.Stream)
            {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEncoding("ascii")
            };

            writer.Write(dig.Magic);
            writer.Write(dig.Type);
            writer.Write(dig.PaletteType);
            writer.Write(dig.PaletteSize);
            writer.Write(dig.Unknown);
            writer.Write(dig.Width);
            writer.Write(dig.Height);

            writer.Write(dig.Palette.GetPalette(0).ToBgr555());
            writer.Write(dig.Pixels.GetData());

            return binary;
        }
    }
}
