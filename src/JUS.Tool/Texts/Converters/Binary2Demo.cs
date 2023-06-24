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
    /// Converts between Demo format and BinaryFormat.
    /// </summary>
    public class Binary2Demo :
        IConverter<BinaryFormat, Demo>,
        IConverter<Demo, BinaryFormat>
    {
        private DataReader reader;

        /// <summary>
        /// Converts BinaryFormat to Demo format.
        /// </summary>
        /// <param name="source">BinaryFormat to convert.</param>
        /// <returns>Text format.</returns>
        /// <exception cref="ArgumentNullException">Source file does not exist.</exception>
        public Demo Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var demo = new Demo();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            demo.Count = reader.ReadInt32();
            for (int i = 0; i < demo.Count; i++) {
                demo.Entries.Add(ReadEntry());
            }

            return demo;
        }

        /// <summary>
        /// Converts Demo format to BinaryFormat.
        /// </summary>
        /// <param name="demo">TextFormat to convert.</param>
        /// <returns>BinaryFormat.</returns>
        public BinaryFormat Convert(Demo demo)
        {
            var bin = new BinaryFormat();
            DataWriter writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new IndirectTextWriter((DemoEntry.EntrySize * demo.Count) + 0x04);

            writer.Write(demo.Count);

            foreach (DemoEntry entry in demo.Entries) {
                JusText.WriteStringPointer(entry.Title, writer, jit);
                JusText.WriteStringPointer(entry.Desc1, writer, jit);
                JusText.WriteStringPointer(entry.Desc2, writer, jit);
                JusText.WriteStringPointer(entry.Desc3, writer, jit);
                writer.Write(entry.Id);
                writer.Write(entry.Icon);
                writer.WritePadding(0x00, 0x04);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        /// <summary>
        /// Reads a single <see cref="DemoEntry"/>.
        /// </summary>
        /// <returns>The read <see cref="DemoEntry"/>.</returns>
        private DemoEntry ReadEntry()
        {
            var entry = new DemoEntry();

            entry.Title = JusText.ReadIndirectString(reader);
            entry.Desc1 = JusText.ReadIndirectString(reader);
            entry.Desc2 = JusText.ReadIndirectString(reader);
            entry.Desc3 = JusText.ReadIndirectString(reader);
            entry.Id = reader.ReadByte();
            entry.Icon = reader.ReadByte();

            reader.SkipPadding(0x04);

            return entry;
        }
    }
}
