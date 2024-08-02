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
using JUSToolkit.Texts.Formats;
using System.Linq;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between Commwin (generic bin) format and Po.
    /// </summary>
    public class Commwin2Po :
        IConverter<Commwin, Po>,
        IConverter<Po, Commwin>
    {
        /// <summary>
        /// Converts Commwin format to Po.
        /// </summary>
        /// <param name="commwin">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(Commwin commwin)
        {
            Po po = JusText.GenerateJusPo();

            int i = 0;
            foreach (string entry in commwin.TextEntries) {
                int extractedComments = commwin.UnknownEntries.ElementAtOrDefault(i); // 0 if the range is out
                po.Add(new PoEntry(JusText.CleanString(entry)) {
                    Context = $"{i++}",
                    ExtractedComments = $"{extractedComments}",
                });
            }

            return po;
        }

        /// <summary>
        /// Converts Po to Commwin format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        public Commwin Convert(Po po)
        {
            var commwin = new Commwin();

            foreach (PoEntry entry in po.Entries) {
                commwin.TextEntries.Add(Table.Instance.Encode(JusText.WriteCleanString(entry.Text)));
                if (int.Parse(entry.ExtractedComments) > 0) {
                    commwin.UnknownEntries.Add(int.Parse(entry.ExtractedComments));
                }
            }

            return commwin;
        }
    }
}
