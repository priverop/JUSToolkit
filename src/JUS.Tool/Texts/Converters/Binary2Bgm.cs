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
    /// Converts between Bgm format and BinaryFormat.
    /// </summary>
    public class Binary2Bgm : IConverter<BinaryFormat, Bgm>, IConverter<Bgm, BinaryFormat>
    {
        private DataReader reader;
        private DataWriter writer;

        /// <summary>
        /// Converts BinaryFormat to Bgm format.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Bgm Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var bgm = new Bgm();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            bgm.Count = reader.ReadInt32();
            for (int i = 0; i < bgm.Count; i++) {
                bgm.Entries.Add(ReadEntry());
            }

            return bgm;
        }

        /// <summary>
        /// Converts Bgm format to BinaryFormat.
        /// </summary>
        public BinaryFormat Convert(Bgm bgm)
        {
            var bin = new BinaryFormat();
            writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new JusIndirectText((BgmEntry.EntrySize * bgm.Count) + 0x04);

            writer.Write(bgm.Count);

            foreach (BgmEntry entry in bgm.Entries) {
                JusText.WriteStringPointer(entry.Title, writer, jit);
                JusText.WriteStringPointer(entry.Desc1, writer, jit);
                JusText.WriteStringPointer(entry.Desc2, writer, jit);
                JusText.WriteStringPointer(entry.Desc3, writer, jit);
                writer.Write(entry.Unk1);
                writer.Write(entry.Unk2);
                writer.Write(entry.Icon);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        /// <summary>
        /// Reads a single <see cref="BgmEntry"/>.
        /// </summary>
        /// <returns>The read <see cref="BgmEntry"/>.</returns>
        private BgmEntry ReadEntry()
        {
            var entry = new BgmEntry();

            entry.Title = JusText.ReadIndirectString(reader);
            entry.Desc1 = JusText.ReadIndirectString(reader);
            entry.Desc2 = JusText.ReadIndirectString(reader);
            entry.Desc3 = JusText.ReadIndirectString(reader);
            entry.Unk1 = reader.ReadInt16();
            entry.Unk2 = reader.ReadInt16();
            entry.Icon = reader.ReadInt32();

            return entry;
        }
    }
}
