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
    /// Converts between Tutorial format and BinaryFormat.
    /// </summary>
    public class Binary2Tutorial :
        IConverter<BinaryFormat, Tutorial>,
        IConverter<Tutorial, BinaryFormat>
    {
        private DataReader reader;
        private int pointerAccumulator = 0;

        /// <summary>
        /// Converts BinaryFormat to Tutorial format.
        /// </summary>
        /// <param name="source">BinaryFormat to convert.</param>
        /// <returns>Text format.</returns>
        /// <exception cref="ArgumentNullException">Source file does not exist.</exception>
        public Tutorial Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var tutorial = new Tutorial();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            reader.Stream.Position = 0x00;
            tutorial.StartingOffset = reader.ReadInt32();

            while (reader.Stream.Position != tutorial.StartingOffset) {
                tutorial.Entries.Add(ReadEntry(tutorial.StartingOffset));
            }

            return tutorial;
        }

        /// <summary>
        /// Converts Tutorial format to BinaryFormat.
        /// </summary>
        /// <param name="tutorial">TextFormat to convert.</param>
        /// <returns>BinaryFormat.</returns>
        public BinaryFormat Convert(Tutorial tutorial)
        {
            var bin = new BinaryFormat();
            DataWriter writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jdt = new DirectTextWriter();

            writer.Write(tutorial.StartingOffset);

            foreach (TutorialEntry entry in tutorial.Entries) {
                foreach (int unknown in entry.Unknowns) {
                    writer.Write(unknown);
                }

                JusText.WriteStringRelativePointer(entry.Description, writer, jdt);
            }

            // Last pointer is not written, let's overwrite it
            writer.Stream.Position -= 4;
            writer.Write(0);

            JusText.WriteAllStrings(writer, jdt);

            return bin;
        }

        /// <summary>
        /// Reads a single <see cref="TutorialEntry"/>.
        /// </summary>
        /// <param name="startingOffset">Accumulator with the sum of the string lengths.</param>
        /// <returns>The read <see cref="TutorialEntry"/>.</returns>
        private TutorialEntry ReadEntry(int startingOffset)
        {
            var entry = new TutorialEntry();
            entry.Description = JusText.ReadIndirectString(reader, startingOffset + pointerAccumulator);

            // +1 is because of the null end byte
            pointerAccumulator += JusText.JusEncoding.GetByteCount(entry.Description) + 1;

            // Read pointers until we find the accumulator one or the ending of the pointer section
            int pointer = reader.ReadInt32();
            while (pointer != pointerAccumulator && reader.Stream.Position != startingOffset) {
                entry.Unknowns.Add(pointer);
                pointer = reader.ReadInt32();
            }

            return entry;
        }
    }
}
