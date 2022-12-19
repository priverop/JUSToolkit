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
    /// Converts between BtlChr format and BinaryFormat.
    /// </summary>
    public class Binary2BtlChr :
        IConverter<BinaryFormat, BtlChr>,
        IConverter<BtlChr, BinaryFormat>
    {
        private DataReader reader;
        private DataWriter writer;

        /// <summary>
        /// Converts BinaryFormat to BtlChr format.
        /// </summary>
        public BtlChr Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var btlChr = new BtlChr();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            int count = reader.ReadInt32() / BtlChrEntry.EntrySize;
            reader.Stream.Position = 0x00;

            for (int i = 0; i < count; i++) {
                btlChr.Entries.Add(ReadEntry());
            }

            return btlChr;
        }

        /// <summary>
        /// Converts BtlChr format to BinaryFormat.
        /// </summary>
        public BinaryFormat Convert(BtlChr btlChr)
        {
            var bin = new BinaryFormat();
            writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new JusIndirectText(BtlChrEntry.EntrySize * btlChr.Entries.Count);

            foreach (BtlChrEntry entry in btlChr.Entries) {
                JusText.WriteStringPointer(entry.ChrName, writer, jit);
                for (int i = 0; i < BtlChrEntry.NumAbilities / 2; i++) {
                    JusText.WriteStringPointer(entry.AbilityNames[i * 2], writer, jit);
                    JusText.WriteStringPointer(entry.AbilityNames[(i * 2) + 1], writer, jit);
                    JusText.WriteStringPointer(entry.AbilityFuriganas[i * 2], writer, jit);
                    JusText.WriteStringPointer(entry.AbilityFuriganas[(i * 2) + 1], writer, jit);
                }

                JusText.WriteStringPointer(entry.PassiveName, writer, jit);
                JusText.WriteStringPointer(entry.PassiveFurigana, writer, jit);
                JusText.WriteStringPointer(entry.PassiveDescription1, writer, jit);
                JusText.WriteStringPointer(entry.PassiveDescription2, writer, jit);

                foreach (string s in entry.AbilityDescriptions) {
                    JusText.WriteStringPointer(s, writer, jit);
                }

                foreach (string s in entry.Interactions) {
                    JusText.WriteStringPointer(s, writer, jit);
                }

                writer.Write(entry.Unk1);
                writer.Write(entry.Unk2);
                writer.Write(entry.Unk3);

                writer.WritePadding(0x00, 0x04);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        private BtlChrEntry ReadEntry()
        {
            var entry = new BtlChrEntry();

            entry.ChrName = JusText.ReadIndirectString(reader);
            for (int i = 0; i < BtlChrEntry.NumAbilities / 2; i++) {
                entry.AbilityNames.Add(JusText.ReadIndirectString(reader));
                entry.AbilityNames.Add(JusText.ReadIndirectString(reader));
                entry.AbilityFuriganas.Add(JusText.ReadIndirectString(reader));
                entry.AbilityFuriganas.Add(JusText.ReadIndirectString(reader));
            }

            entry.PassiveName = JusText.ReadIndirectString(reader);
            entry.PassiveFurigana = JusText.ReadIndirectString(reader);
            entry.PassiveDescription1 = JusText.ReadIndirectString(reader);
            entry.PassiveDescription2 = JusText.ReadIndirectString(reader);

            for (int i = 0; i < BtlChrEntry.NumAbilities * 2; i++) {
                entry.AbilityDescriptions.Add(JusText.ReadIndirectString(reader));
            }

            for (int i = 0; i < BtlChrEntry.NumInteractions * 2; i++) {
                entry.Interactions.Add(JusText.ReadIndirectString(reader));
            }

            entry.Unk1 = reader.ReadInt16();
            entry.Unk2 = reader.ReadInt16();
            entry.Unk3 = reader.ReadInt16();

            reader.SkipPadding(0x04);

            return entry;
        }
    }
}
