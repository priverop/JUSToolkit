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
using Yarhl.FileSystem;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between Container of Deck files and Po.
    /// </summary>
    public class Deck2Po :
        IConverter<NodeContainerFormat, Po>,
        IConverter<Po, NodeContainerFormat>
    {
        /// <summary>
        /// Converts Container of Deck files to Po.
        /// </summary>
        /// <param name="container">Container of Deck files to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(NodeContainerFormat container)
        {
            Po po = JusText.GenerateJusPo();

            foreach (Node file in container.Root.Children) {
                Deck deck = file.GetFormatAs<Deck>();
                po.Add(new PoEntry(deck.Name) {
                    Context = file.Name,
                    ExtractedComments = System.Convert.ToBase64String(deck.Header),
                });
            }

            return po;
        }

        /// <summary>
        /// Converts Po to Container of Deck files format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Container of Deck files.</returns>
        public NodeContainerFormat Convert(Po po)
        {
            var container = new NodeContainerFormat();
            foreach (PoEntry entry in po.Entries) {
                var deck = new Deck() {
                    Name = Table.Instance.Encode(entry.Text),
                    Header = System.Convert.FromBase64String(entry.ExtractedComments),
                };
                container.Root.Add(new Node(entry.Context, deck));
            }

            return container;
        }
    }
}
