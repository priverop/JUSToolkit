using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    public class Bgm2Po :
        IConverter<Bgm, Po>,
        IConverter<Po, Bgm>
    {
        public Po Convert(Bgm bgm)
        {
            var po = JusText.GenerateJusPo();

            int i = 0;
            foreach (BgmEntry entry in bgm.Entries) {
                po.Add(new PoEntry(entry.Title) {
                    Context = $"{i++}",
                    ExtractedComments = $"{entry.Unk1}-{entry.Unk2}-{entry.Icon}",
                });
                string description = $"{entry.Desc1}\n{entry.Desc2}\n{entry.Desc3}";
                po.Add(new PoEntry(description) { Context = $"{i++}", });
            }

            return po;
        }

        public Bgm Convert(Po po)
        {
            var bgm = new Bgm();
            BgmEntry entry;
            List<string> description;
            string[] metadata;

            bgm.Count = po.Entries.Count / 2;

            for (int i = 0; i < bgm.Count; i++) {
                entry = new BgmEntry();
                entry.Title = po.Entries[i * 2].Text;

                description = JusText.SplitStringToList(po.Entries[(i * 2) + 1].Text, '\n', 3);
                entry.Desc1 = description[0];
                entry.Desc2 = description[1];
                entry.Desc3 = description[2];

                metadata = JusText.ParseMetadata(po.Entries[i * 2].ExtractedComments);
                entry.Unk1 = short.Parse(metadata[0]);
                entry.Unk2 = short.Parse(metadata[1]);
                entry.Icon = int.Parse(metadata[2]);

                bgm.Entries.Add(entry);
            }

            return bgm;
        }
    }
}
