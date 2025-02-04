// Copyright (c) 2023 Priverop

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
using System;
using System.Collections.Generic;
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between InfoDeckInfo (generic bin) format and Po.
    /// </summary>
    public class InfoDeckInfo2Po :
        IConverter<InfoDeckInfo, Po>,
        IConverter<Po, InfoDeckInfo>
    {
        /// <summary>
        /// Converts InfoDeckInfo format to Po.
        /// </summary>
        /// <param name="infoDeckInfo">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(InfoDeckInfo infoDeckInfo)
        {
            Po po = JusText.GenerateJusPo();

            int i = 0;
            foreach (InfoDeckEntry entry in infoDeckInfo.Entries) {
                po.Add(new PoEntry(JusText.MergeStrings(entry.Text)) {
                    Context = $"{i++}",
                });
            }

            return po;
        }

        /// <summary>
        /// Converts Po to InfoDeckInfo format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        public InfoDeckInfo Convert(Po po)
        {
            var infoDeck = new InfoDeckInfo();

            InfoDeckEntry entry;
            infoDeck.Count = po.Entries.Count;

            for (int i = 0; i < infoDeck.Count; i++) {
                entry = new InfoDeckEntry();
                List<string> originalLines = JusText.SplitStringToList(po.Entries[i].Original, '\n', infoDeck.LinesPerPage);
                List<string> translatedLines = JusText.SplitStringToList(po.Entries[i].Text, '\n', infoDeck.LinesPerPage);

                if (originalLines.Count != translatedLines.Count) {
                    Console.WriteLine($"Wrong number of lines in {po.Entries[i].Text}");
                    continue;
                }

                foreach (string s in translatedLines) {
                    string sentence = Table.Instance.Encode(JusText.WriteCleanString(s));
                    if (sentence.Length > 38) {
                        Console.WriteLine($"Limit of 38 chars reached in entry {i}: {sentence}");
                        sentence = sentence[0..38];
                    }

                    entry.Text.Add(sentence);
                }

                infoDeck.Entries.Add(entry);
            }

            return infoDeck;
        }
    }
}
