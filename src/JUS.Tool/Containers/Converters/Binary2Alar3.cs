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
using System.IO;
using System.Linq;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Containers.Converters
{
    /// <summary>
    /// Converts between an <see cref="Alar3"/> and a BinaryFormat file.
    /// </summary>
    public class Binary2Alar3 : IConverter<IBinary, Alar3>
    {
        private DataReader reader;
        private Alar3 alar;

        /// <summary>
        /// Converts BinaryFormat to Alar3.
        /// </summary>
        /// <param name="source">File to transform.</param>
        /// <returns>Alar3.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public Alar3 Convert(IBinary source)
        {
            if (source is null) {
                throw new ArgumentNullException(nameof(source));
            }

            reader = new DataReader(source.Stream);
            source.Stream.Position = 0;

            ReadHeader();

            for (int i = 0; i < alar.NumFiles; i++) {
                alar.FileInfoPointers[i] = reader.ReadUInt16();
                source.Stream.RunInPosition(() => ReadFileInfo(source), alar.FileInfoPointers[i]);
            }

            return alar;
        }

        private void ReadHeader()
        {
            string stamp = reader.ReadString(4);
            if (stamp != Alar3.STAMP) {
                throw new FormatException("Invalid header");
            }

            var version = new Version(reader.ReadByte(), reader.ReadByte());
            if (!Alar3.SupportedVersions.Contains(version)) {
                throw new FormatException($"Unsupported version: {version:X}");
            }

            uint numFiles = reader.ReadUInt32();
            ushort reserved = reader.ReadUInt16();
            uint numEntries = reader.ReadUInt32();

            alar = new Alar3(numFiles) {
                NumEntries = numEntries,
                Reserved = reserved,
                DataOffset = reader.ReadUInt16(),
                MinorVersion = (byte)version.Minor,
            };
        }

        private void ReadFileInfo(IBinary source)
        {
            ushort id = reader.ReadUInt16();
            ushort unknown = reader.ReadUInt16();
            uint offset = reader.ReadUInt32();
            uint size = reader.ReadUInt32();

            var fileStream = new DataStream(source.Stream, offset, size);

            var alarFile = new Alar3File(fileStream) {
                FileID = id,
                Unknown = unknown,
                Offset = offset,
                Size = size,
                Unknown2 = reader.ReadUInt16(),
                Unknown3 = reader.ReadUInt16(),
                Unknown4 = reader.ReadUInt16(),
            };

            string path = reader.ReadString();
            string name = Path.GetFileName(path);
            string dir = Path.GetDirectoryName(path);
            var child = new Node(name, alarFile);

            NodeFactory.CreateContainersForChild(alar.Root, dir, child);
        }
    }
}
