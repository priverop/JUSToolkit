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
    /// Converts between Stage format and BinaryFormat.
    /// </summary>
    public class Binary2Stage :
        IConverter<BinaryFormat, Stage>,
        IConverter<Stage, BinaryFormat>
    {
        private DataReader reader;
        private DataWriter writer;

        /// <summary>
        /// Converts BinaryFormat to Stage format.
        /// </summary>
        public Stage Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var stage = new Stage();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            int count = reader.ReadInt32() / StageEntry.EntrySize;
            reader.Stream.Position = 0x00;

            for (int i = 0; i < count; i++) {
                stage.Entries.Add(ReadEntry());
            }

            return stage;
        }

        /// <summary>
        /// Converts Stage format to BinaryFormat.
        /// </summary>
        public BinaryFormat Convert(Stage stage)
        {
            var bin = new BinaryFormat();
            writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new JusIndirectText(StageEntry.EntrySize * stage.Entries.Count);

            foreach (StageEntry entry in stage.Entries) {
                JusText.WriteStringPointer(entry.Name, writer, jit);
                writer.Write(entry.Unk1);
                writer.Write(entry.Unk2);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        /// <summary>
        /// Reads a single <see cref="StageEntry"/>.
        /// </summary>
        /// <returns>The read <see cref="StageEntry"/>.</returns>
        private StageEntry ReadEntry()
        {
            var entry = new StageEntry();

            entry.Name = JusText.ReadIndirectString(reader);
            entry.Unk1 = reader.ReadInt16();
            entry.Unk2 = reader.ReadInt16();

            return entry;
        }
    }
}
