namespace JUSToolkit.Converters.Images
{
    using System;
    using Yarhl.FileFormat;
    using JUSToolkit.Formats;
    using Yarhl.IO;
    using Texim;
    using System.Drawing;
    using log4net;

    public class Binary2DIG :
    IConverter<BinaryFormat, Dig>,
    IConverter<Dig, BinaryFormat>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Identify));

        public Dig Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            DataReader reader = new DataReader(source.Stream);
            reader.Stream.Position = 0;

            Dig dig = new Dig {
                Magic = reader.ReadBytes(4),
                Type = reader.ReadByte(),
                PaletteType = reader.ReadByte(),
                PaletteSize = reader.ReadByte(),
                Unknown = reader.ReadByte(),
                Width = reader.ReadUInt16(),
                Height = reader.ReadUInt16(),
            };

            long paletteEnd = dig.PaletteSize * 32 + reader.Stream.Position;
            dig.PixelsStart = (uint)paletteEnd; // *** 
            log.Debug("Palette End: " + paletteEnd);

            long startPalette = reader.Stream.Position;

            // Si hay bytes vacios antes de empezar la paleta
            if (reader.ReadInt32() == 0) {
                while (reader.ReadInt32() == 0) { }
                startPalette = reader.Stream.Position - 4;
            } else {
                reader.Stream.Position = startPalette;
            }

            dig.PaletteStart = (uint)startPalette;
            log.Debug("Palette Start: " + dig.PaletteStart);

            long paletteActualSize = paletteEnd - dig.PaletteStart;
            log.Debug("Palette Size: " + paletteActualSize);

            reader.Stream.Position = dig.PaletteStart;

            ColorFormat format;

            log.Debug("PaletteType: " + dig.PaletteType);

            if (dig.PaletteType == 16) {
                int paletteColors = 16;
                int paletteSize = paletteColors * 2;
                format = ColorFormat.Indexed_4bpp;
                Color[][] palettes;
                decimal paletteNumber = Math.Ceiling((decimal)paletteActualSize / paletteSize);
                log.Debug("Palette Number: " + paletteNumber);
                palettes = new Color[(int)paletteNumber][];

                for (int i = 0; i < paletteNumber; i++) {
                    palettes[i] = reader.ReadBytes((int)paletteSize).ToBgr555Colors();
                }

                dig.Palette = new Palette(palettes);
            } else {
                format = ColorFormat.Indexed_8bpp;
                dig.Palette = new Palette(reader.ReadBytes((int)paletteActualSize).ToBgr555Colors());

            }
            log.Debug("ColorFormat: " + format);
            dig.Pixels = new PixelArray {
                Width = dig.Width,
                Height = dig.Height,
            };

            int bytesUntilEnd = (int)(reader.Stream.Length - reader.Stream.Position);

            dig.Pixels.SetData(
                reader.ReadBytes(bytesUntilEnd),
                PixelEncoding.HorizontalTiles,
                format,
                new Size(8, 8));

            return dig;
        }

        /// <summary>
        /// Converts a Dig Node to a BinaryFormat Node.
        /// </summary>
        /// <param name="dig">Dig Node.</param>
        /// <returns>BinaryFormat Node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dig"/> is <c>null</c>.</exception>
        public BinaryFormat Convert(Dig dig)
        {
            if (dig == null) {
                throw new ArgumentNullException(nameof(dig));
            }

            var binary = new BinaryFormat();

            var writer = new DataWriter(binary.Stream) {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEncoding("ascii"),
            };

            writer.Write(dig.Magic);
            writer.Write(dig.Type);
            writer.Write(dig.PaletteType);
            writer.Write(dig.PaletteSize);
            writer.Write(dig.Unknown);
            writer.Write(dig.Width);
            writer.Write(dig.Height);

            writer.WriteUntilLength(00, dig.PaletteStart);

            foreach (Color[] c in dig.Palette.GetPalettes()) {
                writer.Write(c.ToBgr555());
            }

            writer.Write(dig.Pixels.GetData());

            return binary;
        }
    }
}
