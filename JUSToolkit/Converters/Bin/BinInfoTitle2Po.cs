using System.Linq;
using JUSToolkit.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Converters.Bin
{
    class BinInfoTitle2Po : IConverter<BinInfoTitle, Po>
    {
        public Po Convert(BinInfoTitle source)
        {
            Po exportPo = new Po()
            {
                Header = new PoHeader("Jump Ultimate Stars", "TranScene", "es")
                {
                    LanguageTeam = "TranScene"
                }
            };

            //Escribir las entradas del Po a partir de la lista de textos
            for(int i = 0; i < source.Text.Count; i++)
            {
                exportPo.Add(new PoEntry(source.Text.ElementAt(i)) { Context = i.ToString()});
            }

            return exportPo;
        }
    }
}
