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
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between BinaryFormat and BinQuiz Format.
    /// </summary>
    public class Binary2BinQuiz : IConverter<BinaryFormat, BinQuiz>
    {
        /// <summary>
        /// Converts BinaryFormat Node to BinQuiz Node.
        /// </summary>
        /// <param name="source">BinaryFormat Node.</param>
        /// <returns>BinQuiz Node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
        public BinQuiz Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var reader = new DataReader(source.Stream) {
                DefaultEncoding = Encoding.GetEncoding("shift_jis"),
            };

            reader.Stream.Position = 0;

            var bin = new BinQuiz {
                Unknown = reader.ReadInt32(),
                Unknown2 = reader.ReadInt32(),
                FirstPointer = reader.ReadInt32(),
            };

            // Vamos al primer puntero (donde está la primera palabra)
            reader.Stream.Position = bin.FirstPointer;
            ReadStringsAddingOffset(bin, reader);

            // Volvemos al principio y leemos los punteros
            reader.Stream.Position = 0;
            ReadPointers(bin, reader);

            return bin;
        }

        /// <summary>
        /// Read all the pointers until the firstPointer
        /// and save them into Pointers Dictionary with its offset.
        /// </summary>
        private static void ReadPointers(BinQuiz bin, DataReader reader)
        {
            while (reader.Stream.Position < bin.FirstPointer) {
                int offset = (int)reader.Stream.Position;
                int pointer = reader.ReadInt32();

                if (bin.Text.ContainsValue(pointer)) {
                    bin.Pointers.Enqueue(pointer);
                    bin.Offsets.Enqueue(offset);
                } else {
                    bin.FillPointers.Enqueue(pointer);
                }
            }
        }

        /// <summary>
        /// Read all the strings until the end of the stream
        /// and save them into Text Dictionary adding the offset.
        /// </summary>
        private static void ReadStringsAddingOffset(BinQuiz bin, DataReader reader)
        {
            int offset = 8;
            int basePointer = bin.FirstPointer + offset;
            reader.Stream.Position = basePointer;

            while (!reader.Stream.EndOfStream) {
                string sentence = reader.ReadString();

                bin.Text.Add(sentence, basePointer);

                offset += 4;

                basePointer += sentence.Length + 1 + offset;
            }
        }
    }
}
