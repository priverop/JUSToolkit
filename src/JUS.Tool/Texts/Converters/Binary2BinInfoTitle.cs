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
    /// Converts between BinaryFormat and BinInfoTitle.
    /// </summary>
    public class Binary2BinInfoTitle : IConverter<BinaryFormat, BinInfoTitle>
    {
        /// <summary>
        /// What is this? @Darkc0m.
        /// </summary>
        /// <param name="reader">DataReader reader to read.</param>
        public static void Go2Text(DataReader reader)
        {
            reader.Stream.Position = 0x00;
            int pointerValue = reader.ReadInt16();

            switch (pointerValue) {
                case 0x0029:
                case 0x005B:
                case 0x0034:
                case 0x0032:
                case 0x0D04:
                case 0x0059:
                    reader.Stream.Position += 2;
                    pointerValue = reader.ReadInt16();
                    break;
                default:
                    break;
            }

            reader.Stream.Position += pointerValue - 2;
        }

        /// <summary>
        /// Converts BinaryFormat to BinInfoTitle Node.
        /// </summary>
        /// <param name="source">BinaryFormat Node.</param>
        /// <returns>BinInfoTitle Node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
        public BinInfoTitle Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var reader = new DataReader(source.Stream) {
                DefaultEncoding = Encoding.GetEncoding(932),
            };

            string sentence;
            var bin = new BinInfoTitle();

            Go2Text(reader);

            while (!reader.Stream.EndOfStream) {
                sentence = reader.ReadString();
                if (string.IsNullOrEmpty(sentence)) {
                    sentence = "<!empty>";
                }

                bin.Text.Add(sentence);
            }

            return bin;
        }
    }
}
