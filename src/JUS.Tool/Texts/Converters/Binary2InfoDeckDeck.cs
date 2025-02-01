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
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between InfoDeckDeck format and BinaryFormat.
    /// </summary>
    public class Binary2InfoDeckDeck :
        IConverter<BinaryFormat, InfoDeckDeck>,
        IConverter<InfoDeckDeck, BinaryFormat>
    {
        private DataReader reader;

        /// <summary>
        /// Converts BinaryFormat to InfoDeckDeck format.
        /// </summary>
        /// <param name="source">BinaryFormat to convert.</param>
        /// <returns>Text format.</returns>
        /// <exception cref="ArgumentNullException">Source file does not exist.</exception>
        public InfoDeckDeck Convert(BinaryFormat source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var infodeckDeck = new InfoDeckDeck();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            infodeckDeck.Count = reader.ReadInt32() / InfoDeckEntry.EntrySize / infodeckDeck.LinesPerPage;
            reader.Stream.Position = 0x00;

            for (int i = 0; i < infodeckDeck.Count; i++) {
                infodeckDeck.Entries.Add(ReadEntry());
            }

            return infodeckDeck;
        }

        /// <summary>
        /// Converts InfoDeckDeck format to BinaryFormat.
        /// </summary>
        /// <param name="infoDeckDeck">TextFormat to convert.</param>
        /// <returns>BinaryFormat.</returns>
        public BinaryFormat Convert(InfoDeckDeck infoDeckDeck)
        {
            var bin = new BinaryFormat();
            var writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new IndirectTextWriter(InfoDeckEntry.EntrySize * infoDeckDeck.Count * infoDeckDeck.LinesPerPage);

            foreach (InfoDeckEntry entry in infoDeckDeck.Entries) {
                foreach (string s in entry.Text) {
                    JusText.WriteStringPointer(s, writer, jit);
                }
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        private InfoDeckEntry ReadEntry()
        {
            var entry = new InfoDeckEntry();
            var infodeckDeck = new InfoDeckDeck();
            for (int i = 0; i < infodeckDeck.LinesPerPage; i++) {
                entry.Text.Add(JusText.ReadIndirectString(reader));
            }

            return entry;
        }
    }
}
