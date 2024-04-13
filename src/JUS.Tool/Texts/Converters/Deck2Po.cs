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
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between Deck format and Po.
    /// </summary>
    public class Deck2Po :
        IConverter<Deck, Po>,
        IConverter<Po, Deck>
    {
        /// <summary>
        /// Converts Deck format to Po.
        /// </summary>
        /// <param name="deck">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(Deck deck)
        {
            Po po = JusText.GenerateJusPo();

            po.Add(new PoEntry(deck.Name) {
                Context = "0",
                ExtractedComments = System.Convert.ToBase64String(deck.Header),
            });

            return po;
        }

        /// <summary>
        /// Converts Po to Deck format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        public Deck Convert(Po po)
        {
            var deck = new Deck();
            deck.Name = po.Entries[0].Text;
            deck.Header = System.Convert.FromBase64String(po.Entries[0].ExtractedComments);
            return deck;
        }
    }
}
