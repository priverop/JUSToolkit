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
    public class Demo2Po :
        IConverter<Demo, Po>,
        IConverter<Po, Demo>
    {
        public Po Convert(Demo demo)
        {
            var po = JusText.GenerateJusPo();

            int i = 0;
            foreach (DemoEntry entry in demo.Entries) {
                po.Add(new PoEntry(entry.Title) {
                    Context = $"{i++}",
                    ExtractedComments = $"{entry.Id}-{entry.Icon}",
                });
                string description = $"{entry.Desc1}\n{entry.Desc2}\n{entry.Desc3}";
                po.Add(new PoEntry(description) { Context = $"{i++}", });
            }

            return po;
        }

        public Demo Convert(Po po)
        {
            var demo = new Demo();
            DemoEntry entry;
            List<string> description;
            string[] metadata;

            demo.Count = po.Entries.Count / 2;

            for (int i = 0; i < demo.Count; i++) {
                entry = new DemoEntry();
                entry.Title = po.Entries[i * 2].Text;

                description = JusText.SplitStringToList(po.Entries[(i * 2) + 1].Text, '\n', 3);
                entry.Desc1 = description[0];
                entry.Desc2 = description[1];
                entry.Desc3 = description[2];

                metadata = JusText.ParseMetadata(po.Entries[i * 2].ExtractedComments);
                entry.Id = byte.Parse(metadata[0]);
                entry.Icon = byte.Parse(metadata[1]);

                demo.Entries.Add(entry);
            }

            return demo;
        }
    }
}
