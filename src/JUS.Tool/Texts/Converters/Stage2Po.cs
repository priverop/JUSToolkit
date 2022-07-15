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
    public class Stage2Po :
        IConverter<Stage, Po>,
        IConverter<Po, Stage>
    {
        public Po Convert(Stage stage)
        {
            var po = JusText.GenerateJusPo();

            int i = 0;
            foreach (StageEntry entry in stage.Entries) {
                po.Add(new PoEntry(entry.Name) {
                    Context = $"{i++}",
                    ExtractedComments = $"{entry.Unk1}-{entry.Unk2}",
                });
            }

            return po;
        }

        public Stage Convert(Po po)
        {
            var stage = new Stage();
            StageEntry entry;
            string[] metadata;

            for (int i = 0; i < po.Entries.Count; i++) {
                entry = new StageEntry();
                entry.Name = po.Entries[i].Text;

                metadata = JusText.ParseMetadata(po.Entries[i].ExtractedComments);
                entry.Unk1 = short.Parse(metadata[0]);
                entry.Unk2 = short.Parse(metadata[1]);

                stage.Entries.Add(entry);
            }

            return stage;
        }
    }
}
