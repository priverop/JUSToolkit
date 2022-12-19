// Copyright (c) 2021 Darkc0m

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System.Collections.Generic;
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between BtlChr format and Po.
    /// </summary>
    public class BtlChr2Po :
        IConverter<BtlChr, Po>,
        IConverter<Po, BtlChr>
    {
        /// <summary>
        /// Converts BtlChr format to Po.
        /// </summary>
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

        /// <summary>
        /// Converts Po to BtlChr format.
        /// </summary>
        public BtlChr Convert(Po po)
        {
            var btlChr = new BtlChr();
            BtlChrEntry entry;
            List<string> splitText;
            string[] metadata;

            for (int i = 0; i < po.Entries.Count / 26; i++) {
                entry = new BtlChrEntry();

                entry.ChrName = po.Entries[i * 26].Text;
                splitText = JusText.SplitStringToList(po.Entries[(i * 26) + 1].Text, '\n', 2);
                entry.PassiveName = splitText[0];
                entry.PassiveFurigana = splitText[1];
                splitText = JusText.SplitStringToList(po.Entries[(i * 26) + 2].Text, '\n', 2);
                entry.PassiveDescription1 = splitText[0];
                entry.PassiveDescription2 = splitText[1];

                for (int j = 0; j < 10; j++) {
                    splitText = JusText.SplitStringToList(po.Entries[(i * 26) + 3 + (j * 2)].Text, '\n', 2);
                    entry.AbilityNames.Add(splitText[0]);
                    entry.AbilityFuriganas.Add(splitText[1]);
                    splitText = JusText.SplitStringToList(po.Entries[(i * 26) + 4 + (j * 2)].Text, '\n', 2);
                    entry.AbilityDescriptions.Add(splitText[0]);
                    entry.AbilityDescriptions.Add(splitText[1]);
                }

                for (int j = 23; j < 26; j++) {
                    splitText = JusText.SplitStringToList(po.Entries[(i * 26) + j].Text, '\n', 2);
                    entry.Interactions.Add(splitText[0]);
                    entry.Interactions.Add(splitText[1]);
                }

                metadata = JusText.ParseMetadata(po.Entries[i * 26].ExtractedComments);
                entry.Unk1 = short.Parse(metadata[0]);
                entry.Unk2 = short.Parse(metadata[1]);
                entry.Unk3 = short.Parse(metadata[2]);

                btlChr.Entries.Add(entry);
            }

            return btlChr;
        }
    }
}
