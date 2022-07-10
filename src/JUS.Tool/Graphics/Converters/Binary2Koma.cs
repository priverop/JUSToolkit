// Copyright (c) 2021 SceneGate

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
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Graphics.Converters
{
    /// <summary>
    /// Converts between BinaryFormat and Koma.
    /// </summary>
    public class Binary2Koma : IConverter<BinaryFormat, Koma>
    {
        private const int EntrySize = 12;

        /// <summary>
        /// Converts a BinaryFormat (file) to a Koma Node.
        /// </summary>
        /// <param name="source">BinaryFormat (file) to convert.</param>
        /// <returns>Koma Node.</returns>
        public Koma Convert(BinaryFormat source)
        {
            if (source is null) {
                throw new ArgumentNullException(nameof(source));
            }

            var koma = new Koma();
            var reader = new DataReader(source.Stream);
            source.Stream.Position = 0;

            int numEntries = (int)(source.Stream.Length / EntrySize);
            for (int i = 0; i < numEntries; i++) {
                var element = new KomaElement {
                    ImageId = reader.ReadUInt16(),
                    Unknown2 = reader.ReadUInt16(),
                };

                byte nameIdx = reader.ReadByte();
                byte nameNum = reader.ReadByte();
                element.KomaName = $"{Koma.NameTable[nameIdx]}_{nameNum:D2}";

                element.Unknown6 = reader.ReadByte();
                element.Unknown7 = reader.ReadByte();
                element.KShapeGroupId = reader.ReadByte();
                element.KShapeElementId = reader.ReadByte();
                element.UnknownA = reader.ReadByte();
                element.UnknownB = reader.ReadByte();

                koma.Add(element);
            }

            return koma;
        }
    }
}
