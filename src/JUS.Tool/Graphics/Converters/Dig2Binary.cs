using System;
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
            writer.Write(dig.NumPalettes);
            writer.Write(dig.Width);
            writer.Write(dig.Height);

            // Quizá luego: writer.WriteUntilLength(00, dig.PaletteStart);
            foreach (IPalette c in dig.PaletteCollection.Palettes) {
                writer.Write<Bgr555>(c.Colors);
            }

            IIndexedPixelEncoding encoder = (dig.ImageFormat == 0x10)
                ? Indexed4Bpp.Instance
                : Indexed8Bpp.Instance;

            writer.Write(encoder.Encode(dig.Pixels));

            return binary;
        }
    }
}
