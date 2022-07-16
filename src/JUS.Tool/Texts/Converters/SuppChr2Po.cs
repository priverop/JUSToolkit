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
    public class SuppChr2Po :
        IConverter<SuppChr, Po>,
        IConverter<Po, SuppChr>
    {
        public Po Convert(SuppChr suppChr)
        {
            var po = JusText.GenerateJusPo();

            int i = 0;
            foreach (SuppChrEntry entry in suppChr.Entries) {
                po.Add(new PoEntry(entry.chrName) { Context = $"{i++}" });

                for (int j = 0; j < SuppChrEntry.NumAbilities; j++) {
                    po.Add(new PoEntry($"{entry.Abilities[j * 2]}\n{entry.Abilities[(j * 2) + 1]}") { Context = $"{i++}" });
                    po.Add(new PoEntry($"{entry.Descriptions[j * 2]}\n{entry.Descriptions[(j * 2) + 1]}") { Context = $"{i++}" });
                }
            }

            return po;
        }

        public SuppChr Convert(Po po)
        {
            var suppChr = new SuppChr();
            SuppChrEntry entry;
            string[] splitText;

            for (int i = 0; i < po.Entries.Count / 5; i++) {
                entry = new SuppChrEntry();
                entry.chrName = po.Entries[i * 5].Text;

                for (int j = 0; j < SuppChrEntry.NumAbilities; j++) {
                    splitText = JusText.SplitStringToFixedSizeArray(po.Entries[(i * 5) + 1 + (j * 2)].Text, '\n', 2);
                    entry.Abilities.Add(splitText[0]);
                    entry.Abilities.Add(splitText[1]);

                    splitText = JusText.SplitStringToFixedSizeArray(po.Entries[(i * 5) + 2 + (j * 2)].Text, '\n', 2);
                    entry.Descriptions.Add(splitText[0]);
                    entry.Descriptions.Add(splitText[1]);
                }

                suppChr.Entries.Add(entry);
            }

            return suppChr;
        }
    }
}
