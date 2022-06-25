// Copyright (c) 2022 Pablo Rivero
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between BinaryFormat to BinTutorial Node.
    /// </summary>
    public class BinaryFormat2BinTutorial : IConverter<BinaryFormat, BinTutorial>
    {
        /// <summary>
        /// Converts a BinaryFormat to a BinTutorial Node.
        /// </summary>
        /// <param name="source">BinaryFormat Node.</param>
        /// <returns>BinTutorial Node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
        public BinTutorial Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var reader = new DataReader(source.Stream) {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEncoding("ascii"),
            };

            var bin = new BinTutorial {
                FirstPointer = reader.ReadInt32(),
            };

            reader.Stream.Position = bin.FirstPointer;
            ReadStrings(bin, reader);
            reader.Stream.Position = 4; // skip first pointer
            ReadPointers(bin, reader);

            return bin;
        }

        /// <summary>
        /// Read all the strings until the end of the stream
        /// and save them into Text Dictionary with its Length +1 (null char).
        /// </summary>
        private static void ReadStrings(BinTutorial bin, DataReader reader)
        {
            int actualPointer = 0;
            while (!reader.Stream.EndOfStream) {
                string sentence = reader.ReadString();
                actualPointer += sentence.Length + 1; // \0 char
                bin.Text.Add(sentence, actualPointer);
            }
        }

        /// <summary>
        /// Read all the pointers until the firstPointer
        /// and save them into Pointers Dictionary with its offset.
        /// </summary>
        private static void ReadPointers(BinTutorial bin, DataReader reader)
        {
            while (reader.Stream.Position < bin.FirstPointer) {
                int offset = (int)reader.Stream.Position;
                int pointer = reader.ReadInt32();

                if (bin.Text.ContainsValue(pointer) && !bin.Pointers.ContainsKey(pointer)) {
                    bin.Pointers.Add(pointer, offset);
                } else {
                    bin.FillPointers.Enqueue(pointer);
                }
            }
        }
    }
}
