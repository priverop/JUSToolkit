// Copyright (c) 2022 Priverop

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
    /// Converts between Rulemess format and Po.
    /// </summary>
    public class Rulemess2Po :
        IConverter<Rulemess, Po>,
        IConverter<Po, Rulemess>
    {
        /// <summary>
        /// Converts Rulemess format to Po.
        /// </summary>
        /// <param name="rulemess">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(Rulemess rulemess)
        {
            var po = JusText.GenerateJusPo();

            int i = 0;
            foreach (RulemessEntry entry in rulemess.Entries) {
                string description = $"{entry.Description1}\n{entry.Description2}\n{entry.Description3}";
                po.Add(new PoEntry(description) {
                    Context = $"{i++}",
                    ExtractedComments = $"{entry.Unk1}",
                });
            }

            return po;
        }

        /// <summary>
        /// Converts Po to Rulemess format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        public Rulemess Convert(Po po)
        {
            var rulemess = new Rulemess();
            RulemessEntry entry;
            List<string> description;

            for (int i = 0; i < po.Entries.Count; i++) {
                entry = new RulemessEntry();
                description = JusText.SplitStringToList(po.Entries[i].Text, '\n', 3);
                entry.Description1 = description[0];
                entry.Description2 = description[1];
                entry.Description3 = description[2];

                entry.Unk1 = int.Parse(po.Entries[i].ExtractedComments);

                rulemess.Entries.Add(entry);
            }

            return rulemess;
        }
    }
}
