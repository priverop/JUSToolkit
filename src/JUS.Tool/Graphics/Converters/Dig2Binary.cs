using System;
using System.Drawing.Imaging;
using Texim.Colors;
using Texim.Palettes;
using Texim.Pixels;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Graphics.Converters
{
    /// <summary>
    /// Converts between BinaryFile and a <see cref="Dig"/> Node.
    /// </summary>
    public class Dig2Binary :
    IConverter<Dig, BinaryFormat>
    {
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

            var writer = new DataWriter(binary.Stream);

            writer.Write(Dig.STAMP, false);
            writer.Write(dig.Unknown);
            writer.Write(dig.ImageFormat);
            writer.Write(dig.NumPaletteLines);
            writer.Write((ushort)dig.Width);
            writer.Write((ushort)dig.Height);

            foreach (IPalette c in dig.Palettes) {
                writer.Write<Bgr555>(c.Colors);
            }

            IIndexedPixelEncoding encoder = dig.Bpp switch {
                DigBpp.Bpp4 => Indexed4Bpp.Instance,
                DigBpp.Bpp8 => Indexed8Bpp.Instance,
                _ => throw new FormatException("Invalid bpp"),
            };

            writer.Stream.Position = dig.PixelsStart;
            IndexedPixel[] pixels = dig.Swizzling switch {
                DigSwizzling.Linear => dig.Pixels,
                DigSwizzling.Tiled => new TileSwizzling<IndexedPixel>(dig.Width).Swizzle(dig.Pixels),
                _ => throw new FormatException("Invalid swizzling"),
            };
            writer.Write(encoder.Encode(pixels));

            return binary;
        }
    }
}
