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
using JUSToolkit.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Converters.Bin
{
    /// <summary>
    /// Converts between BinaryFile to BinFilename Format.
    /// </summary>
    public class Binary2BinFilename : IConverter<BinaryFormat, BinFilename>
    {
        /// <summary>
        /// Converts BinaryFormat into BinFilename Node.
        /// </summary>
        /// <param name="source">BinaryFormat Node.</param>
        /// <returns>BinFilename Node.</returns>
        public BinFilename Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var bin = new BinFilename();

            var reader = new DataReader(source.Stream) {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEncoding("ascii"),
            };

            // Guardamos estos dos para compararlos y sacar el tipo, no haría falta
            long currentPosition = reader.Stream.Position;
            int firstPointer = reader.ReadInt32();
            int secondPointer = reader.ReadInt32();
            reader.Stream.Position = currentPosition;

            // Vamos al primer puntero (donde está la primera palabra)
            reader.Stream.Position = firstPointer;
            ReadStringsAddingOffset(reader);

            // Volvemos al principio y leemos los punteros
            reader.Stream.Position = 0;
            this.ReadPointers(reader);

            return bin;
        }

        /// <summary>
        /// Read all the strings until the end of the stream
        /// and save them into Text Dictionary with its additional Length +1 (null char).
        /// ActualPointer saves up the total length.
        /// </summary>
        private void ReadStringsAddingLength(DataReader fileToExtractReader)
        {
            int actualPointer = 0;
            while (!fileToExtractReader.Stream.EndOfStream) {
                string sentence = fileToExtractReader.ReadString();
                actualPointer += sentence.Length + 1; // \0 char
                //Text.Add(sentence, actualPointer);
            }
        }

        /// <summary>
        /// Reads all the strings until the end of the stream
        /// and save them into Text Dictionary adding the offset.
        /// </summary>
        private static void ReadStringsAddingOffset(DataReader fileToExtractReader)
        {
            int offset = 0;
            // int basePointer = this.FirstPointer;

            while (!fileToExtractReader.Stream.EndOfStream) {
                string sentence = fileToExtractReader.ReadString();

                // Text.Add(sentence, basePointer - offset);

                // basePointer += sentence.Length + 1;
                offset += 4;
            }
        }

        /// <summary>
        /// Read all the pointers until the firstPointer
        /// and save them into Pointers Dictionary with its offset.
        /// </summary>
        private void ReadPointers(DataReader fileToExtractReader)
        {
            //while (fileToExtractReader.Stream.Position < FirstPointer)
            //{
            //    int offset = (int)fileToExtractReader.Stream.Position;
            //    int pointer = fileToExtractReader.ReadInt32();

            //    if (Text.ContainsValue(pointer))
            //    {
            //        Pointers.Enqueue(pointer);
            //        Offsets.Enqueue(offset);
            //    }
            //    else
            //        FillPointers.Enqueue(pointer);
            //}
        }
    }
}
