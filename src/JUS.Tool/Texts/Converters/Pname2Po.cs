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
    public class Pname2Po :
        IConverter<Pname, Po>,
        IConverter<Po, Pname>
    {
        public Po Convert(Pname pname)
        {
            var po = JusText.GenerateJusPo();

            int i = 0;
            foreach (string entry in pname.TextEntries) {
                po.Add(new PoEntry(entry) {
                    Context = $"{i++}",
                });
            }

            return po;
        }

        public Pname Convert(Po po)
        {
            var pname = new Pname();

            pname.Count = po.Entries.Count;
            foreach (PoEntry entry in po.Entries) {
                pname.TextEntries.Add(entry.Text);
            }

            return pname;
        }
    }
}
