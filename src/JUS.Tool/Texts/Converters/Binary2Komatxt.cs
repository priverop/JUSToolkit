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
using System;
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between Komatxt format and BinaryFormat.
    /// </summary>
    public class Binary2Komatxt :
        IConverter<BinaryFormat, Komatxt>,
        IConverter<Komatxt, BinaryFormat>
    {
        private DataReader reader;

        /// <summary>
        /// Converts BinaryFormat to Komatxt format.
        /// </summary>
        /// <param name="source">BinaryFormat to convert.</param>
        /// <returns>Text format.</returns>
        /// <exception cref="ArgumentNullException">Source file does not exist.</exception>
        public Komatxt Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var komatxt = new Komatxt();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            int count = reader.ReadInt32() / KomatxtEntry.EntrySize;
            reader.Stream.Position = 0x00;

            for (int i = 0; i < count; i++) {
                komatxt.Entries.Add(ReadEntry());
            }

            return komatxt;
        }

        /// <summary>
        /// Converts Komatxt format to BinaryFormat.
        /// </summary>
        /// <param name="komatxt">TextFormat to convert.</param>
        /// <returns>BinaryFormat.</returns>
        public BinaryFormat Convert(Komatxt komatxt)
        {
            var bin = new BinaryFormat();
            DataWriter writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new IndirectTextWriter(KomatxtEntry.EntrySize * komatxt.Entries.Count);

            foreach (KomatxtEntry entry in komatxt.Entries) {
                JusText.WriteStringPointer(entry.Name, writer, jit);
                writer.Write(entry.Unk1);
                writer.Write(entry.Unk2);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        /// <summary>
        /// Reads a single <see cref="KomatxtEntry"/>.
        /// </summary>
        /// <returns>The read <see cref="KomatxtEntry"/>.</returns>
        private KomatxtEntry ReadEntry()
        {
            var entry = new KomatxtEntry();

            entry.Name = JusText.ReadIndirectString(reader);
            entry.Unk1 = reader.ReadInt32();
            entry.Unk2 = reader.ReadInt32();

            return entry;
        }
    }
}
