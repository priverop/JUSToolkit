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
    /// Converts between InfoDeck format and BinaryFormat.
    /// </summary>
    public class Binary2InfoDeck :
        IConverter<BinaryFormat, InfoDeck>,
        IConverter<InfoDeck, BinaryFormat>
    {
        private DataReader reader;

        /// <summary>
        /// Converts BinaryFormat to InfoDeck format.
        /// </summary>
        /// <param name="source">BinaryFormat to convert.</param>
        /// <returns>Text format.</returns>
        /// <exception cref="ArgumentNullException">Source file does not exist.</exception>
        public InfoDeck Convert(BinaryFormat source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var infodeck = new InfoDeck();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            infodeck.Count = reader.ReadInt32() / InfoDeckEntry.EntrySize / InfoDeckEntry.LinesPerPage;
            reader.Stream.Position = 0x00;

            for (int i = 0; i < infodeck.Count; i++) {
                infodeck.Entries.Add(ReadEntry());
            }

            return infodeck;
        }

        /// <summary>
        /// Converts InfoDeck format to BinaryFormat.
        /// </summary>
        /// <param name="infoDeck">TextFormat to convert.</param>
        /// <returns>BinaryFormat.</returns>
        public BinaryFormat Convert(InfoDeck infoDeck)
        {
            var bin = new BinaryFormat();
            var writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new IndirectTextWriter(InfoDeckEntry.EntrySize * infoDeck.Count);

            foreach (InfoDeckEntry entry in infoDeck.Entries) {
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
            for (int i = 0; i < InfoDeckEntry.LinesPerPage; i++) {
                entry.Text.Add(JusText.ReadIndirectString(reader));
            }

            return entry;
        }
    }
}
