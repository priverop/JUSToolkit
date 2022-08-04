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
using System.IO;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Containers.Converters
{
    /// <summary>
    /// Converter between BinaryFormat and Alar2.
    /// </summary>
    public class Binary2Alar2 : IConverter<IBinary, Alar2>
    {
        private DataReader reader;
        private Alar2 alar;

        /// <summary>
        /// Converts a BinaryFormat to an Alar2 container.
        /// </summary>
        /// <param name="input">IBinary node.</param>
        /// <returns>Alart2 NodeContainerFormat.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> is <c>null</c>.</exception>
        public Alar2 Convert(IBinary input) {
            if (input == null) {
                throw new ArgumentNullException(nameof(input));
            }

            reader = new DataReader(input.Stream);

            ReadHeader();

            uint name_offset = (uint)(0x10 + (alar.NumFiles * 0x10));
            for (int i = 0; i < alar.NumFiles; i++)
            {
                uint fileID = reader.ReadUInt32();
                uint offset = reader.ReadUInt32();
                uint size = reader.ReadUInt32();
                uint unknown = reader.ReadUInt32();

                var fileStream = new DataStream(input.Stream, offset, size);

                var alarFile = new Alar2File(fileStream) {
                    FileID = fileID,
                    Offset = offset,
                    Size = size,
                    Unknown = unknown,
                };

                input.Stream.RunInPosition(() => alarFile.Unknown2 = reader.ReadUInt16(), offset - 2);
                input.Stream.RunInPosition(() => ReadFileInfo(alarFile), name_offset + 2);

                name_offset += size + 0x24;
            }

            return alar;
        }

        private void ReadHeader()
        {
            string stamp = reader.ReadString(4);
            if (stamp != Alar2.STAMP) {
                throw new FormatException("Invalid header");
            }

            var version = new Version(reader.ReadByte(), reader.ReadByte());
            if (version != Alar2.SupportedVersion) {
                throw new FormatException($"Unsupported version: {version:X}");
            }

            alar = new Alar2 {
                NumFiles = reader.ReadUInt16(),
                IDs = new byte[8],
            };

            for (int i = 0; i < 8; i++) {
                alar.IDs[i] = reader.ReadByte();
            }
        }

        private void ReadFileInfo(Alar2File alarFile)
        {
            string filename = reader.ReadString();

            var child = new Node(filename, alarFile);

            alar.Root.Add(child);
        }
    }
}
