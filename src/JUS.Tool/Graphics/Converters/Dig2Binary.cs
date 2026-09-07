using JUS.Tool.Framework;
using Texim.Colors;
using Texim.Palettes;
using Texim.Pixels;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUS.Tool.Graphics.Converters
{
    /// <summary>
    /// Converts a <see cref="Dig"/> image into its binary format.
    /// </summary>
    public class Dig2Binary :
    IConverter<Dig, BinaryFormat>
    {
        /// <summary>
        /// Converts a dig to a binary format.
        /// </summary>
        /// <param name="dig">Dig format.</param>
        /// <returns>Binary format from memory.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dig"/> is <c>null</c>.</exception>
        public BinaryFormat Convert(Dig dig)
        {
            ArgumentNullException.ThrowIfNull(dig);

            var binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream);

            writer.Write(Dig.Stamp, false);
            writer.Write(dig.Version);

            int flags = ((int)dig.DataFormat << 4) | (int)dig.Bpp;
            writer.Write((byte)flags);

            int formatPaletteCount = dig.Bpp == DigBpp.Bpp4
                ? dig.Palettes.Count
                : (int)Math.Ceiling(dig.Palettes[0].Colors.Count / 16.0);
            writer.Write((byte)formatPaletteCount);

            writer.Write(dig.UnknownValue7);

            if (dig.DataFormat is DigDataFormat.CompressedBlocks) {
                writer.WriteTimes(0x00, 4); // placeholder
            } else {
                writer.Write((ushort)dig.OriginalSize.Width);
                writer.Write((ushort)dig.OriginalSize.Height);
            }

            foreach (IPalette c in dig.Palettes) {
                writer.Write<Abgr555Encoding>(c.Colors);
            }

            if (dig.DataFormat is DigDataFormat.CompressedBlocks) {
                if (dig.Version != 1) {
                    writer.Write(dig.UnknownBlockValue);
                }

                WriteCompressedBlocks(writer, dig);
            } else {
                WriteIndexedPixels(writer, dig);
            }

            return binary;
        }

        private static void WriteIndexedPixels(DataWriter writer, Dig dig)
        {
            IIndexedPixelEncoding encoder = dig.Bpp switch {
                DigBpp.Bpp4 => Indexed4BppEncoding.Instance,
                DigBpp.Bpp8 => Indexed8BppEncoding.Instance,
                _ => throw new FormatException("Invalid bpp"),
            };

            // TODO: compress for format compressed image
            IndexedPixel[] pixels = dig.DataFormat switch {
                DigDataFormat.Linear or DigDataFormat.CompressedBlocks => dig.Pixels,
                DigDataFormat.Tiled => new TileSwizzling<IndexedPixel>(dig.Width).Swizzle(dig.Pixels),
                _ => throw new FormatException("Invalid format"),
            };
            writer.Write(encoder.Encode(pixels));
        }

        private static void WriteCompressedBlocks(DataWriter writer, Dig dig)
        {
            long basePosition = writer.Stream.Position;
            writer.Write(dig.CompressedSegments.Length);

            // prefill table
            writer.WriteTimes(0x00, 4 * dig.CompressedSegments.Length);
            writer.Stream.Position = basePosition + 4;

            // Write table entry and block
            foreach (byte[] block in dig.CompressedSegments) {
                ushort encodedOffset = (ushort)((writer.Stream.Length - basePosition) / 4);
                writer.Write(encodedOffset);
                writer.Write((ushort)block.Length);

                using (writer.Stream.EnterWithPosition(0, SeekOrigin.End)) {
                    writer.Write(block);
                    writer.WritePadding(0x00, 4);
                }
            }

            int infoLength = 4 + (4 * dig.CompressedSegments.Length);
            int dataLength = (int)(writer.Stream.Length - basePosition + 4);

            writer.Stream.Position = 0x08;
            writer.Write((ushort)(infoLength / 4));
            writer.Write((ushort)(dataLength / 4));
        }
    }
}
