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
    /// Converts between Ability format and BinaryFormat.
    /// </summary>
    public class Binary2Ability : IConverter<BinaryFormat, Ability>, IConverter<Ability, BinaryFormat>
    {
        private DataReader reader;

        /// <summary>
        /// Converts BinaryFormat to Ability format.
        /// </summary>
        /// <param name="source">BinaryFormat to convert.</param>
        /// <returns>Text format.</returns>
        /// <exception cref="ArgumentNullException">Source file does not exist.</exception>
        public Ability Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var ability = new Ability();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            ability.Count = reader.ReadInt32() / AbilityEntry.EntrySize;
            reader.Stream.Position = 0x00;

            for (int i = 0; i < ability.Count; i++) {
                ability.Entries.Add(ReadEntry());
            }

            return ability;
        }

        /// <summary>
        /// Converts Ability format to BinaryFormat.
        /// </summary>
        /// <param name="ability">TextFormat to convert.</param>
        /// <returns>BinaryFormat.</returns>
        public BinaryFormat Convert(Ability ability)
        {
            var bin = new BinaryFormat();
            DataWriter writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new IndirectTextWriter(AbilityEntry.EntrySize * ability.Count);

            foreach (AbilityEntry entry in ability.Entries) {
                JusText.WriteStringPointer(entry.Title, writer, jit);
                JusText.WriteStringPointer(entry.Description1, writer, jit);
                JusText.WriteStringPointer(entry.Description2, writer, jit);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        /// <summary>
        /// Reads a single <see cref="AbilityEntry"/>.
        /// </summary>
        /// <returns>The read <see cref="AbilityEntry"/>.</returns>
        private AbilityEntry ReadEntry()
        {
            var entry = new AbilityEntry();

            entry.Title = JusText.ReadIndirectString(reader);
            entry.Description1 = JusText.ReadIndirectString(reader);
            entry.Description2 = JusText.ReadIndirectString(reader);

            return entry;
        }
    }
}
