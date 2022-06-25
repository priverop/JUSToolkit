using JUSToolkit.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Converters.Bin
{
    /// <summary>
    /// Converts between BinInfoTitle and Po.
    /// </summary>
    public class BinInfoTitle2Po : IConverter<BinInfoTitle, Po>
    {
        /// <summary>
        /// Converts a BinaryInfoTitle Node to a Po Node.
        /// </summary>
        /// <param name="source">BinInfoTitle Node.</param>
        /// <returns>Po Node.</returns>
        public Po Convert(BinInfoTitle source)
        {
            var exportPo = new Po()
            {
                Header = new PoHeader("Jump Ultimate Stars", "TranScene", "es")
                {
                    LanguageTeam = "TranScene",
                },
            };

            int i = 0;

            // Escribo las entradas del Po a partir de la lista de textos
            foreach (string entry in source.Text)
            {
                exportPo.Add(new PoEntry(entry) { Context = i.ToString() });
                i++;
            }

            return exportPo;
        }
    }
}
