using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUS.Tool.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUS.Tool.Texts.Converters
{
    public class SimpleBin2Po :
        IConverter<SimpleBin, Po>,
        IConverter<Po, SimpleBin>
    {
        public Po Convert(SimpleBin simpleBin)
        {
            var po = JusText.GenerateJusPo();

            int i = 0;
            foreach (string entry in simpleBin.TextEntries) {
                po.Add(new PoEntry(entry) {
                    Context = $"{i++}",
                });
            }

            return po;
        }

        public SimpleBin Convert(Po po)
        {
            var simpleBin = new SimpleBin();

            foreach (PoEntry entry in po.Entries) {
                simpleBin.TextEntries.Add(entry.Text);
            }

            return simpleBin;
        }
    }
}
