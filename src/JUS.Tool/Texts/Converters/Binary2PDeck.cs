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
    /// Converts between PDeck format and BinaryFormat.
    /// </summary>
    public class Binary2PDeck :
        IConverter<BinaryFormat, PDeck>,
        IConverter<PDeck, BinaryFormat>
    {
        /// <summary>
        /// Converts BinaryFormat to PDeck format.
        /// </summary>
        /// <param name="source">BinaryFormat to convert.</param>
        /// <returns>Text format.</returns>
        /// <exception cref="ArgumentNullException">Source file does not exist.</exception>
        public PDeck Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var pDeck = new PDeck();
            DataReader reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            reader.Stream.Position = 0x00;
            pDeck.Header = reader.ReadBytes(pDeck.HeaderSize);

            int nameSize = pDeck.UnkownPosition - pDeck.HeaderSize;
            pDeck.Name = reader.ReadString(nameSize).TrimEnd('\0');
            reader.Stream.Position = pDeck.UnkownPosition;
            pDeck.Unknown = reader.ReadInt32();

            return pDeck;
        }

        /// <summary>
        /// Converts PDeck format to BinaryFormat.
        /// </summary>
        /// <param name="pDeck">TextFormat to convert.</param>
        /// <returns>BinaryFormat.</returns>
        public BinaryFormat Convert(PDeck pDeck)
        {
            var bin = new BinaryFormat();
            DataWriter writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            writer.Write(pDeck.Header);
            writer.Write(pDeck.Name);
            writer.WriteUntilLength(0, pDeck.UnkownPosition);
            writer.Write(pDeck.Unknown);
            writer.WriteUntilLength(0, pDeck.FileSize);

            return bin;
        }
    }
}
