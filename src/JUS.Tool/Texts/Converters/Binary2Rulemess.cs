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
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between Rulemess format and BinaryFormat.
    /// </summary>
    public class Binary2Rulemess :
        IConverter<BinaryFormat, Rulemess>,
        IConverter<Rulemess, BinaryFormat>
    {
        private DataReader reader;
        private DataWriter writer;

        /// <summary>
        /// Converts BinaryFormat to Rulemess format.
        /// </summary>
        /// <param name="source">BinaryFormat to convert.</param>
        /// <returns>Text format.</returns>
        /// <exception cref="ArgumentNullException">Source file does not exist.</exception>
        public Rulemess Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var rulemess = new Rulemess();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            int count = reader.ReadInt32() / RulemessEntry.EntrySize;
            reader.Stream.Position = 0x00;

            for (int i = 0; i < count; i++) {
                rulemess.Entries.Add(ReadEntry());
            }

            return rulemess;
        }

        /// <summary>
        /// Converts Rulemess format to BinaryFormat.
        /// </summary>
        /// <param name="rulemess">TextFormat to convert.</param>
        /// <returns>BinaryFormat.</returns>
        public BinaryFormat Convert(Rulemess rulemess)
        {
            var bin = new BinaryFormat();
            writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new IndirectTextWriter(RulemessEntry.EntrySize * rulemess.Entries.Count);

            foreach (RulemessEntry entry in rulemess.Entries) {
                JusText.WriteStringPointer(entry.Description1, writer, jit);
                JusText.WriteStringPointer(entry.Description2, writer, jit);
                JusText.WriteStringPointer(entry.Description3, writer, jit);
                writer.Write(entry.Unk1);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        /// <summary>
        /// Reads a single <see cref="RulemessEntry"/>.
        /// </summary>
        /// <returns>The read <see cref="RulemessEntry"/>.</returns>
        private RulemessEntry ReadEntry()
        {
            var entry = new RulemessEntry();

            entry.Description1 = JusText.ReadIndirectString(reader);
            entry.Description2 = JusText.ReadIndirectString(reader);
            entry.Description3 = JusText.ReadIndirectString(reader);
            entry.Unk1 = reader.ReadInt32();

            return entry;
        }
    }
}
