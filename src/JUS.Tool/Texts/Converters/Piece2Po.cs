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
    public class Piece2Po :
        IConverter<Piece, Po>,
        IConverter<Po, Piece>
    {
        public Po Convert(Piece piece)
        {
            var po = JusText.GenerateJusPo();

            int i = 0;
            foreach (PieceEntry entry in piece.Entries) {
                po.Add(new PoEntry(entry.Title) {
                    Context = $"{i++}",
                    ExtractedComments = $"{entry.Unk1}-{entry.Id}",
                });
                po.Add(new PoEntry(JusText.MergeStrings(entry.Authors)) { Context = $"{i++}" });
                po.Add(new PoEntry(JusText.MergeStrings(entry.Info)) { Context = $"{i++}" });
                po.Add(new PoEntry(JusText.MergeStrings(entry.Page1)) { Context = $"{i++}" });
                po.Add(new PoEntry(JusText.MergeStrings(entry.Page2)) { Context = $"{i++}" });
            }

            return po;
        }

        public Piece Convert(Po po)
        {
            var piece = new Piece();
            PieceEntry entry;
            List<string> splitString;
            string[] metadata;

            piece.Count = po.Entries.Count / 5;

            for (int i = 0; i < piece.Count; i++) {
                entry = new PieceEntry();
                entry.Title = po.Entries[i * 5].Text;

                foreach (string s in JusText.SplitStringToList(po.Entries[(i * 5) + 1].Text, '\n', 2)) {
                    entry.Authors.Add(s);
                }

                foreach (string s in JusText.SplitStringToList(po.Entries[(i * 5) + 2].Text, '\n', 2)) {
                    entry.Info.Add(s);
                }

                foreach (string s in JusText.SplitStringToList(po.Entries[(i * 5) + 3].Text, '\n', 9)) {
                    entry.Page1.Add(s);
                }

                foreach (string s in JusText.SplitStringToList(po.Entries[(i * 5) + 4].Text, '\n', 9)) {
                    entry.Page2.Add(s);
                }

                metadata = JusText.ParseMetadata(po.Entries[i * 5].ExtractedComments);
                entry.Unk1 = short.Parse(metadata[0]);
                entry.Id = short.Parse(metadata[1]);

                piece.Entries.Add(entry);
            }

            return piece;
        }
    }
}
