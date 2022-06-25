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
using JUSToolkit.Formats.ALAR;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Converters.Alar
{
    /// <summary>
    /// Converter between BinaryFormat and Alar2.
    /// </summary>
    public class BinaryFormat2Alar2 : IConverter<BinaryFormat, Alar2>
    {
        /// <summary>
        /// Converts a BinaryFormat to an Alar2 container.
        /// </summary>
        /// <param name="input">BinaryFormat node.</param>
        /// <returns>Alart2 NodeContainerFormat.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> is <c>null</c>.</exception>
        public Alar2 Convert(BinaryFormat input) {
            if (input == null) {
                throw new ArgumentNullException(nameof(input));
            }

            input.Stream.Seek(0, SeekMode.Start); // Just in case

            var br = new DataReader(input.Stream)
            {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEncoding("ascii"),
            };

            var aar = new Alar2 {
                Header = br.ReadChars(4),
                Type = br.ReadByte(),
                Unk = br.ReadByte(),
                Num_files = br.ReadUInt16(),
                IDs = new byte[8],
            };

            for (int i = 0; i < 8; i++) {
                aar.IDs[i] = br.ReadByte();
            }

            // Index table
            uint name_offset = (uint)(0x10 + (aar.Num_files * 0x10));
            for (int i = 0; i < aar.Num_files; i++)
            {
                var aarFile = new Alar2File();
                uint unk1 = br.ReadUInt32();
                uint offset = br.ReadUInt32();
                uint size = br.ReadUInt32();
                uint unk2 = br.ReadUInt32();

                var fileStream = new DataStream(input.Stream, offset, size);

                long curPos = br.Stream.Position;
                br.Stream.Position = name_offset + 2;
                string filename = br.ReadString();
                name_offset += size + 0x24;
                br.Stream.Position = curPos;

                aarFile.File = new Node(filename, new BinaryFormat(fileStream));

                aar.AlarFiles.Add(aarFile);
            }

            br.Stream.Dispose();

            return aar;
        }
    }
}
