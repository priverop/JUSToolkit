using JUSToolkit.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;
using System.Collections.Generic;

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

            int i = 0;
            //Escribo las entradas del Po a partir de la lista de textos
            foreach(string entry in source.Text)
            {
                
                exportPo.Add(new PoEntry(entry) { Context = i.ToString() });
                i++;
            }

            return exportPo;
        }
    }
}
