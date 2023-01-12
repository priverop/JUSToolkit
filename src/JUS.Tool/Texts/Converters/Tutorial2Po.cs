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
    /// Converts between Tutorial format and Po.
    /// </summary>
    public class Tutorial2Po :
        IConverter<Tutorial, Po>,
        IConverter<Po, Tutorial>
    {
        /// <summary>
        /// Converts Tutorial format to Po.
        /// </summary>
        /// <param name="tutorial">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(Tutorial tutorial)
        {
            var po = JusText.GenerateJusPo();
            po.Add(new PoEntry("<!Don't remove>") {
                ExtractedComments = $"{tutorial.StartingOffset}",
            });

            int i = 0;
            foreach (TutorialEntry entry in tutorial.Entries) {
                po.Add(new PoEntry(entry.Description) {
                    Context = $"{i++}",
                    ExtractedComments = JusText.MergeStrings(entry.Unknowns),
                });
            }

            return po;
        }

        /// <summary>
        /// Converts Po to Tutorial format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        public Tutorial Convert(Po po)
        {
            var tutorial = new Tutorial();
            TutorialEntry entry;
            string[] metadata;

            tutorial.StartingOffset = int.Parse(po.Entries[0].ExtractedComments);

            for (int i = 1; i < po.Entries.Count; i++) {
                entry = new TutorialEntry();
                entry.Description = po.Entries[i].Text;

                metadata = JusText.ParseMetadata(po.Entries[i].ExtractedComments);
                entry.Unknowns = metadata.Select(int.Parse).ToList();

                tutorial.Entries.Add(entry);
            }

            return tutorial;
        }
    }
}
