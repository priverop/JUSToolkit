using System.Text;
using JUSToolkit.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Converters.Bin
{
    /// <summary>
    /// Converts between a <see cref="BinaryFormat"/> and a <see cref="BinInfoTitle"/>.
    /// </summary>
    public class BinInfoTitle2Bin : IConverter<BinInfoTitle, BinaryFormat>
    {
        /// <summary>
        /// Converts a BinInfoTitle to a BinaryFormat.
        /// </summary>
        /// <param name="source">BinInfoTitle Node.</param>
        /// <returns>BinaryFormat Node.</returns>
        public BinaryFormat Convert(BinInfoTitle source)
        {
            var bin = new BinaryFormat();

            var writer = new DataWriter(bin.Stream)
            {
                DefaultEncoding = Encoding.GetEncoding(932),
            };

            foreach (int pointer in source.Pointers)
            {
                writer.WriteOfType((short)pointer);
            }

            foreach (string text in source.Text)
            {
                writer.Write(text);
            }

            return bin;
        }
    }
}
