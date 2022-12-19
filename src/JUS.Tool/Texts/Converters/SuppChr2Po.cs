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
    /// Converts between SuppChr format and Po.
    /// </summary>
    public class SuppChr2Po :
        IConverter<SuppChr, Po>,
        IConverter<Po, SuppChr>
    {
        /// <summary>
        /// Converts SuppChr format to Po.
        /// </summary>
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

        /// <summary>
        /// Converts Po to SuppChr format.
        /// </summary>
        public SuppChr Convert(Po po)
        {
            var suppChr = new SuppChr();
            SuppChrEntry entry;
            List<string> splitText;

            for (int i = 0; i < po.Entries.Count / 5; i++) {
                entry = new SuppChrEntry();
                entry.chrName = po.Entries[i * 5].Text;

                for (int j = 0; j < SuppChrEntry.NumAbilities; j++) {
                    splitText = JusText.SplitStringToList(po.Entries[(i * 5) + 1 + (j * 2)].Text, '\n', 2);
                    entry.Abilities.Add(splitText[0]);
                    entry.Abilities.Add(splitText[1]);

                    splitText = JusText.SplitStringToList(po.Entries[(i * 5) + 2 + (j * 2)].Text, '\n', 2);
                    entry.Descriptions.Add(splitText[0]);
                    entry.Descriptions.Add(splitText[1]);
                }

                suppChr.Entries.Add(entry);
            }

            return suppChr;
        }
    }
}
