using System;
using System.Drawing;
using JUSToolkit.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Converters.Images
{
    /// <summary>
    /// Converts between BinaryFile and a <see cref="Dig"/> Node.
    /// </summary>
    public class Binary2Dig :
    IConverter<IBinary, Dig>,
    IConverter<Dig, BinaryFormat>
    {
        /// <summary>
        /// Converts BinaryFormat to a Dig Node.
        /// </summary>
        /// <param name="source">BinaryFormat Node.</param>
        /// <returns>Dig Node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
        public Dig Convert(IBinary source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

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

            long paletteEnd = (dig.PaletteSize * 32) + reader.Stream.Position;
            dig.PixelsStart = (uint) paletteEnd;

            long startPalette = reader.Stream.Position;

            // Si hay bytes vacios antes de empezar la paleta
            if (reader.ReadInt32() == 0) {
                while (reader.ReadInt32() == 0);
                startPalette = reader.Stream.Position - 4;
            } else {
                reader.Stream.Position = startPalette;
            }

            dig.PaletteStart = (uint)startPalette;

            long paletteActualSize = paletteEnd - dig.PaletteStart;

            reader.Stream.Position = dig.PaletteStart;

            ColorFormat format;

            if (dig.PaletteType == 16) {
                const int paletteColors = 16;
                const int paletteSize = paletteColors * 2;
                format = ColorFormat.Indexed_4bpp;
                Color[][] palettes;
                decimal paletteNumber = Math.Ceiling((decimal)paletteActualSize / paletteSize);

                palettes = new Color[(int)paletteNumber][];

                for (int i = 0; i < paletteNumber; i++) {
                    palettes[i] = reader.ReadBytes((int)paletteSize).ToBgr555Colors();
                }

                dig.Palette = new Palette(palettes);
            } else {
                format = ColorFormat.Indexed_8bpp;
                dig.Palette = new Palette(reader.ReadBytes((int)paletteActualSize).ToBgr555Colors());

            }

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
