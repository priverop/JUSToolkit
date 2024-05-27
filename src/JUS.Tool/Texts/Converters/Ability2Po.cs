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
    /// Converts between Ability format and Po.
    /// </summary>
    public class Ability2Po :
        IConverter<Ability, Po>,
        IConverter<Po, Ability>
    {
        /// <summary>
        /// Converts Ability format to Po.
        /// </summary>
        /// <param name="ability">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(Ability ability)
        {
            Po po = JusText.GenerateJusPo();

            int i = 0;
            foreach (AbilityEntry entry in ability.Entries) {
                po.Add(new PoEntry(entry.Title) {
                    Context = $"{i++}",
                });
                string description = string.IsNullOrWhiteSpace(entry.Description1) ?
                                    "<!empty>" :
                                    $"{entry.Description1}\n{entry.Description2}";

                po.Add(new PoEntry(description.TrimEnd()) { Context = $"{i++}", });
            }

            return po;
        }

        /// <summary>
        /// Converts Po to Ability format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        public Ability Convert(Po po)
        {
            var ability = new Ability();
            AbilityEntry entry;
            List<string> description;

            ability.Count = po.Entries.Count / 2;

            for (int i = 0; i < ability.Count; i++) {
                entry = new AbilityEntry();

                entry.Title = po.Entries[i * 2].Text;

                string descriptionEntry = po.Entries[(i * 2) + 1].Text;
                if (descriptionEntry == "<!empty>") {
                    entry.Description1 = string.Empty;
                    entry.Description2 = string.Empty;
                } else {
                    description = JusText.SplitStringToList(descriptionEntry, '\n', 2);
                    entry.Description1 = description[0];
                    entry.Description2 = description[1];
                }

                ability.Entries.Add(entry);
            }

            return ability;
        }
    }
}
