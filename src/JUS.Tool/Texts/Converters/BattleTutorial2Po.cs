// Copyright (c) 2022 Pablo Rivero

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
using System.Linq;
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between BattleTutorial format and Po.
    /// </summary>
    public class BattleTutorial2Po :
        IConverter<BattleTutorial, Po>,
        IConverter<Po, BattleTutorial>
    {
        /// <summary>
        /// Converts BattleTutorial format to Po.
        /// </summary>
        /// <param name="battleTutorial">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(BattleTutorial battleTutorial)
        {
            var po = JusText.GenerateJusPo();
            po.Add(new PoEntry("<!Don't remove>") {
                ExtractedComments = $"{battleTutorial.StartingOffset}",
            });

            int i = 0;
            foreach (BattleTutorialEntry entry in battleTutorial.Entries) {
                po.Add(new PoEntry(entry.Description) {
                    Context = $"{i++}",
                    ExtractedComments = JusText.MergeStrings(entry.Unknowns),
                });
            }

            return po;
        }

        /// <summary>
        /// Converts Po to BattleTutorial format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        public BattleTutorial Convert(Po po)
        {
            var battleTutorial = new BattleTutorial();
            BattleTutorialEntry entry;
            string[] metadata;

            battleTutorial.StartingOffset = int.Parse(po.Entries[0].ExtractedComments);

            for (int i = 1; i < po.Entries.Count; i++) {
                entry = new BattleTutorialEntry();
                entry.Description = po.Entries[i].Text;

                metadata = JusText.ParseMetadata(po.Entries[i].ExtractedComments);
                entry.Unknowns = metadata.Select(int.Parse).ToList();

                battleTutorial.Entries.Add(entry);
            }

            return battleTutorial;
        }
    }
}
