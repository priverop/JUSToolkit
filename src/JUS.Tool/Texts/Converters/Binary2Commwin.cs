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
    /// Converts between Commwin format and BinaryFormat.
    /// </summary>
    public class Binary2Commwin :
        IConverter<BinaryFormat, Commwin>,
        IConverter<Commwin, BinaryFormat>
    {
        /// <summary>
        /// Converts BinaryFormat to Commwin format.
        /// </summary>
        /// <param name="source">BinaryFormat to convert.</param>
        /// <returns>Text format.</returns>
        /// <exception cref="ArgumentNullException">Source file does not exist.</exception>
        public Commwin Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var simple = new Commwin();
            var reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            reader.Stream.Position = 0x04;
            int count = (reader.ReadInt32() + 4) / Commwin.EntrySize; // we add 4 because the pointer position is 0x04 and it's an absolute pointer
            reader.Stream.Position = 0x00;

            for (int i = 0; i < count; i++) {
                if (i == 0 || i % 9 == 0) {
                    simple.UnknownEntries.Add(reader.ReadInt32());
                } else {
                    simple.TextEntries.Add(JusText.ReadIndirectString(reader));
                }
            }

            return simple;
        }

        /// <summary>
        /// Converts Commwin format to BinaryFormat.
        /// </summary>
        /// <param name="commwin">TextFormat to convert.</param>
        /// <returns>BinaryFormat.</returns>
        public BinaryFormat Convert(Commwin commwin)
        {
            var bin = new BinaryFormat();
            var writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new IndirectTextWriter(Commwin.EntrySize * (commwin.TextEntries.Count + commwin.UnknownEntries.Count));

            int i = 0;
            int j = 0;

            foreach (string entry in commwin.TextEntries) {
                if (i == 0 || i % 8 == 0) {
                    writer.Write(commwin.UnknownEntries[j]);
                    j++;
                }
                JusText.WriteStringPointer(entry, writer, jit);
                i++;
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }
    }
}
