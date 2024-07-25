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
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between SimpleBin (generic bin) format and Po.
    /// </summary>
    public class SimpleBin2Po :
        IConverter<SimpleBin, Po>,
        IConverter<Po, SimpleBin>
    {
        /// <summary>
        /// Converts SimpleBin format to Po.
        /// </summary>
        /// <param name="simpleBin">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(SimpleBin simpleBin)
        {
            Po po = JusText.GenerateJusPo();

            int i = 0;
            foreach (string entry in simpleBin.TextEntries) {
                po.Add(new PoEntry(entry) {
                    Context = $"{i++}",
                });
            }

            return po;
        }

        /// <summary>
        /// Converts Po to SimpleBin format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        public SimpleBin Convert(Po po)
        {
            var simpleBin = new SimpleBin();

            foreach (PoEntry entry in po.Entries) {
                simpleBin.TextEntries.Add(Table.Instance.Encode(entry.Text));
            }

            return simpleBin;
        }
    }
}
