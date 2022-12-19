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
using System;
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between SuppChr format and BinaryFormat.
    /// </summary>
    public class Binary2SuppChr :
        IConverter<BinaryFormat, SuppChr>,
        IConverter<SuppChr, BinaryFormat>
    {
        private DataReader reader;
        private DataWriter writer;

        /// <summary>
        /// Converts BinaryFormat to SuppChr format.
        /// </summary>
        public SuppChr Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var suppChr = new SuppChr();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            int count = reader.ReadInt32() / SuppChrEntry.EntrySize;
            reader.Stream.Position = 0x00;

            for (int i = 0; i < count; i++) {
                suppChr.Entries.Add(ReadEntry());
            }

            return suppChr;
        }

        /// <summary>
        /// Converts SuppChr format to BinaryFormat.
        /// </summary>
        public BinaryFormat Convert(SuppChr suppChr)
        {
            var bin = new BinaryFormat();
            writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new JusIndirectText(SuppChrEntry.EntrySize * suppChr.Entries.Count);

            foreach (SuppChrEntry entry in suppChr.Entries) {
                JusText.WriteStringPointer(entry.chrName, writer, jit);
                foreach (string s in entry.Abilities) {
                    JusText.WriteStringPointer(s, writer, jit);
                }

                foreach (string s in entry.Descriptions) {
                    JusText.WriteStringPointer(s, writer, jit);
                }
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        private SuppChrEntry ReadEntry()
        {
            var entry = new SuppChrEntry();

            entry.chrName = JusText.ReadIndirectString(reader);

            for (int i = 0; i < SuppChrEntry.NumAbilities * 2; i++) {
                entry.Abilities.Add(JusText.ReadIndirectString(reader));
            }

            for (int i = 0; i < SuppChrEntry.NumAbilities * 2; i++) {
                entry.Descriptions.Add(JusText.ReadIndirectString(reader));
            }

            return entry;
        }
    }
}
