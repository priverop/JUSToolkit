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
    public class BtlChr2Po :
        IConverter<BtlChr, Po>,
        IConverter<Po, BtlChr>
    {
        public Po Convert(BtlChr btlChr)
        {
            var po = JusText.GenerateJusPo();

            int i = 0;
            foreach (BtlChrEntry entry in btlChr.Entries) {
                po.Add(new PoEntry(entry.ChrName) {
                    Context = $"{i++}",
                    ExtractedComments = $"{entry.Unk1}-{entry.Unk2}-{entry.Unk3}",
                });

                po.Add(new PoEntry($"{entry.PassiveName}\n{entry.PassiveFurigana}") {
                    Context = $"{i++}",
                });

                po.Add(new PoEntry($"{entry.PassiveDescription1}\n{entry.PassiveDescription2}") {
                    Context = $"{i++}",
                });

                for (int j = 0; j < BtlChrEntry.NumAbilities; j++) {
                    po.Add(new PoEntry($"{entry.AbilityNames[j]}\n{entry.AbilityFuriganas[j]}") {
                        Context = $"{i++}",
                    });
                    po.Add(new PoEntry($"{entry.AbilityDescriptions[j * 2]}\n{entry.AbilityDescriptions[(j * 2) + 1]}") {
                        Context = $"{i++}",
                    });
                }

                for (int j = 0; j < BtlChrEntry.NumInteractions; j++) {
                    po.Add(new PoEntry($"{entry.Interactions[j * 2]}\n{entry.Interactions[(j * 2) + 1]}") {
                        Context = $"{i++}",
                    });
                }
            }

            return po;
        }

        public BtlChr Convert(Po po)
        {
            var btlChr = new BtlChr();
            BtlChrEntry entry;
            string[] splitText;

            for (int i = 0; i < po.Entries.Count / 26; i++) {
                entry = new BtlChrEntry();

                entry.ChrName = po.Entries[i * 26].Text;
                splitText = JusText.SplitStringToFixedSizeArray(po.Entries[(i * 26) + 1].Text, '\n', 2);
                entry.PassiveName = splitText[0];
                entry.PassiveFurigana = splitText[1];
                splitText = JusText.SplitStringToFixedSizeArray(po.Entries[(i * 26) + 2].Text, '\n', 2);
                entry.PassiveDescription1 = splitText[0];
                entry.PassiveDescription2 = splitText[1];

                for (int j = 0; j < 10; j++) {
                    splitText = JusText.SplitStringToFixedSizeArray(po.Entries[(i * 26) + 3 + (j * 2)].Text, '\n', 2);
                    entry.AbilityNames.Add(splitText[0]);
                    entry.AbilityFuriganas.Add(splitText[1]);
                    splitText = JusText.SplitStringToFixedSizeArray(po.Entries[(i * 26) + 4 + (j * 2)].Text, '\n', 2);
                    entry.AbilityDescriptions.Add(splitText[0]);
                    entry.AbilityDescriptions.Add(splitText[1]);
                }

                for (int j = 23; j < 26; j++) {
                    splitText = JusText.SplitStringToFixedSizeArray(po.Entries[(i * 26) + j].Text, '\n', 2);
                    entry.Interactions.Add(splitText[0]);
                    entry.Interactions.Add(splitText[1]);
                }

                splitText = JusText.ParseMetadata(po.Entries[i * 26].ExtractedComments);
                entry.Unk1 = short.Parse(splitText[0]);
                entry.Unk2 = short.Parse(splitText[1]);
                entry.Unk3 = short.Parse(splitText[2]);

                btlChr.Entries.Add(entry);
            }

            return btlChr;
        }
    }
}
