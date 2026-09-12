using SceneGate.Ekona.Compression;
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

            WriteHeader(writer, dig);
            WritePalettes(writer, dig);

            if (dig.DataFormat is DigDataFormat.CompressedBlocks) {
                if (dig.Version != 1) {
                    writer.Write(dig.UnknownBlockValue);
                }

                WriteCompressedBlocks(writer, dig);
            } else {
                WriteIndexedPixels(writer.Stream, dig);
            }

            return binary;
        }

        private static void WriteHeader(DataWriter writer, Dig dig)
        {
            writer.Write(Dig.Stamp, false);
            writer.Write(dig.Version);

            int flags = ((int)dig.DataFormat << 4) | (int)dig.Bpp;
            writer.Write((byte)flags);

            int formatPaletteCount = dig.Bpp == DigBpp.Bpp4
                ? dig.Palettes.Count
                : (int)Math.Ceiling(dig.Palettes[0].Colors.Count / 16.0);
            writer.Write((byte)formatPaletteCount);
            writer.Write((byte)dig.FormatColorEncoding);

            if (dig.DataFormat is DigDataFormat.CompressedBlocks) {
                writer.WriteTimes(0x00, 4); // placeholder for block infos
            } else if (dig.HasValidSize) {
                writer.Write((ushort)dig.Width);
                writer.Write((ushort)dig.Height);
            } else {
                // Ignore current image size, and write what it was originally
                writer.Write((ushort)dig.OriginalSize.Width);
                writer.Write((ushort)dig.OriginalSize.Height);
            }
        }

        private static void WritePalettes(DataWriter writer, Dig dig)
        {
            IColorEncoding colorEncoding = dig.ActualColorEncodingFormat.GetColorEncoding();
            foreach (IPalette c in dig.Palettes) {
                byte[] encodedColors = colorEncoding.Encode(c.Colors);
                writer.Write(encodedColors);
            }
        }

        private static void WriteIndexedPixels(Stream stream, Dig dig)
        {
            IndexedPixel[] pixels = dig.DataFormat is DigDataFormat.Tiled
                ? new TileSwizzling<IndexedPixel>(dig.Width).Swizzle(dig.Pixels)
                : dig.Pixels;
            byte[] encodedPixels = dig.Bpp.GetPixelEncoding().Encode(pixels);

            if (dig.DataFormat is DigDataFormat.CompressedImage) {
                using var inputCompression = new MemoryStream(encodedPixels);
                using Stream outputCompression = new LzssEncoder().Convert(inputCompression);
                outputCompression.WriteTo(stream);
            } else {
                stream.Write(encodedPixels);
            }
        }

        private static void WriteCompressedBlocks(DataWriter writer, Dig dig)
        {
            long basePosition = writer.Stream.Position;
            writer.Write(dig.BlocksInfo.Length);

            // prefill table
            writer.WriteTimes(0x00, 4 * dig.BlocksInfo.Length);
            writer.Stream.Position = basePosition + 4;

            // Write table entry and block
            foreach (DigBlockInfo block in dig.BlocksInfo) {
                // Compress
                ReadOnlySpan<IndexedPixel> blockPixels = dig.Pixels.AsSpan(block.PixelStart, block.PixelCount);
                byte[] encodedPixels = dig.Bpp.GetPixelEncoding().Encode(blockPixels);
                using var inputCompression = new MemoryStream(encodedPixels);
                using Stream outputCompression = new LzssEncoder().Convert(inputCompression);

                ushort encodedOffset = (ushort)((writer.Stream.Length - basePosition) / 4);
                writer.Write(encodedOffset);
                writer.Write((ushort)outputCompression.Length);

                using (writer.Stream.EnterWithPosition(0, SeekOrigin.End)) {
                    outputCompression.WriteTo(writer.Stream);
                    writer.WritePadding(0x00, 4);
                }
            }

            int infoLength = 4 + (4 * dig.BlocksInfo.Length);
            int dataLength = (int)(writer.Stream.Length - basePosition);
            if (dig.Version != 1) {
                dataLength += 4;
            }

            writer.Stream.Position = 0x08;
            writer.Write((ushort)(infoLength / 4));
            writer.Write((ushort)(dataLength / 4));
        }
    }
}
