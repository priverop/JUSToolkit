// Copyright (c) 2023 Priverop

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
    /// Converts between JGalaxyComplex format and BinaryFormat.
    /// </summary>
    public class Binary2JGalaxyComplex :
        IConverter<BinaryFormat, JGalaxyComplex>
        // IConverter<JGalaxyComplex, BinaryFormat>
    {
        private DataReader reader;

        /// <summary>
        /// Converts BinaryFormat to JGalaxyComplex format.
        /// </summary>
        /// <param name="source">BinaryFormat to convert.</param>
        /// <returns>Text format.</returns>
        /// <exception cref="ArgumentNullException">Source file does not exist.</exception>
        public JGalaxyComplex Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var jgalaxy = new JGalaxyComplex();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            reader.Stream.Position = 0x00;
            short numberOfEntries0 = reader.ReadInt16();
            short numberOfEntries1 = reader.ReadInt16();
            short numberOfEntries2 = reader.ReadInt16();
            short numberOfEntries3 = reader.ReadInt16();

            int startingPointer0 = reader.ReadInt32();
            int startingPointer1 = reader.ReadInt32();
            int startingPointer2 = reader.ReadInt32();
            int startingPointer3 = reader.ReadInt32();

            int blockSize0 = startingPointer1 - startingPointer0;
            int blockSize1 = startingPointer3 - startingPointer1;
            int blockSize2 = (int)source.Stream.Length - startingPointer2;
            int blockSize3 = startingPointer2 - startingPointer3;

            jgalaxy.Blocks[0] = ReadBlock(numberOfEntries0, startingPointer0, blockSize0);
            jgalaxy.Blocks[1] = ReadBlock(numberOfEntries1, startingPointer1, blockSize1);

            // This file has the 4th block before the 3rd.
            jgalaxy.Blocks[3] = ReadBlock(numberOfEntries3, startingPointer3, blockSize3);
            jgalaxy.Blocks[2] = ReadBlock(numberOfEntries2, startingPointer2, blockSize2);

            return jgalaxy;
        }

        private JGalaxyComplexBlock ReadBlock(short numberOfEntries, int startingPointer, int blockSize)
        {

            if (reader.Stream.Position != startingPointer) {
                throw new Exception("Issue reading the file: wrong position of the block.");
            }

            int entrySize = blockSize / numberOfEntries;
            var block = new JGalaxyComplexBlock(numberOfEntries, startingPointer, entrySize);

            for (int i = 0; i < numberOfEntries; i++) {
                block.Entries.Add(ReadEntry(entrySize));
            }

            return block;
        }

        /// <summary>
        /// Reads a single <see cref="JGalaxyEntry"/>.
        /// </summary>
        /// <returns>The read <see cref="JGalaxyEntry"/>.</returns>
        private JGalaxyEntry ReadEntry(int entrySize)
        {
            var entry = new JGalaxyEntry {
                EntrySize = entrySize,
                Description = reader.ReadString(),
            };

            // Skipping zeros
            int zeroCounter = 0;
            while (reader.ReadByte() == 0) {
                zeroCounter++;
            }

            reader.Stream.Position--; // Because the while did read a non zero value

            int unknownLength = entrySize - entry.Description.Length - zeroCounter - 1;

            entry.Unknown = reader.ReadBytes(unknownLength);

            return entry;
        }

        /// <summary>
        /// Converts JGalaxyComplex format to BinaryFormat.
        /// </summary>
        /// <param name="jgalaxy">TextFormat to convert.</param>
        /// <returns>BinaryFormat.</returns>
        // public BinaryFormat Convert(JGalaxyComplex jgalaxy)
        // {
        //     var bin = new BinaryFormat();
        //     var writer = new DataWriter(bin.Stream) {
        //         DefaultEncoding = JusText.JusEncoding,
        //     };

        //     writer.Write(jgalaxy.NumberOfEntries);

        //     foreach (JGalaxyComplexEntry entry in jgalaxy.Entries) {
        //         writer.Write(entry.Description);

        //         // I don't know if the extra byte if because of the null ending string or is just the length, but don't remove it
        //         long numberOfZeros = JGalaxyComplexEntry.EntrySize - entry.Description.Length - entry.Unknown.Length - 1;

        //         writer.WriteTimes(00, numberOfZeros);
        //         writer.Write(entry.Unknown);
        //     }

        //     return bin;
        // }
    }
}
