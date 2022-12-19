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
    /// Converts between Piece format and BinaryFormat.
    /// </summary>
    public class Binary2Piece :
        IConverter<BinaryFormat, Piece>,
        IConverter<Piece, BinaryFormat>
    {
        private DataReader reader;
        private DataWriter writer;

        /// <summary>
        /// Converts BinaryFormat to Piece format.
        /// </summary>
        /// <param name="source">BinaryFormat to convert.</param>
        /// <returns>Text format.</returns>
        /// <exception cref="ArgumentNullException">Source file does not exist.</exception>
        public Piece Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var piece = new Piece();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            piece.Count = reader.ReadInt32();
            for (int i = 0; i < piece.Count; i++) {
                piece.Entries.Add(ReadEntry());
            }

            return piece;
        }

        /// <summary>
        /// Converts Piece format to BinaryFormat.
        /// </summary>
        /// <param name="piece">TextFormat to convert.</param>
        /// <returns>BinaryFormat.</returns>
        public BinaryFormat Convert(Piece piece)
        {
            var bin = new BinaryFormat();
            writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new IndirectTextWriter((PieceEntry.EntrySize * piece.Count) + 0x04);

            writer.Write(piece.Count);

            foreach (PieceEntry entry in piece.Entries) {
                JusText.WriteStringPointer(entry.Title, writer, jit);
                foreach (string s in entry.Authors) {
                    JusText.WriteStringPointer(s, writer, jit);
                }

                foreach (string s in entry.Info) {
                    JusText.WriteStringPointer(s, writer, jit);
                }

                foreach (string s in entry.Page1) {
                    JusText.WriteStringPointer(s, writer, jit);
                }

                foreach (string s in entry.Page2) {
                    JusText.WriteStringPointer(s, writer, jit);
                }

                writer.Write(entry.Unk1);
                writer.Write(entry.Id);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        private PieceEntry ReadEntry()
        {
            var entry = new PieceEntry();

            entry.Title = JusText.ReadIndirectString(reader);
            for (int i = 0; i < PieceEntry.NumAuthors; i++) {
                entry.Authors.Add(JusText.ReadIndirectString(reader));
            }

            for (int i = 0; i < PieceEntry.NumInfo; i++) {
                entry.Info.Add(JusText.ReadIndirectString(reader));
            }

            for (int i = 0; i < PieceEntry.LinesPerPage; i++) {
                entry.Page1.Add(JusText.ReadIndirectString(reader));
            }

            for (int i = 0; i < PieceEntry.LinesPerPage; i++) {
                entry.Page2.Add(JusText.ReadIndirectString(reader));
            }

            entry.Unk1 = reader.ReadInt16();
            entry.Id = reader.ReadInt16();

            return entry;
        }
    }
}
