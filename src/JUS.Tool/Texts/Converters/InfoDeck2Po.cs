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
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between InfoDeck (generic bin) format and Po.
    /// </summary>
    public class InfoDeck2Po :
        IConverter<InfoDeck, Po>,
        IConverter<Po, InfoDeck>
    {
        /// <summary>
        /// Converts InfoDeck format to Po.
        /// </summary>
        /// <param name="InfoDeck">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(InfoDeck InfoDeck)
        {
            var po = JusText.GenerateJusPo();

            int i = 0;
            foreach (string entry in InfoDeck.TextEntries) {
                po.Add(new PoEntry(entry) {
                    Context = $"{i++}",
                });
            }

            return po;
        }

        /// <summary>
        /// Converts Po to InfoDeck format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        public InfoDeck Convert(Po po)
        {
            var InfoDeck = new InfoDeck();

            foreach (PoEntry entry in po.Entries) {
                InfoDeck.TextEntries.Add(entry.Text);
            }

            return InfoDeck;
        }
    }
}
