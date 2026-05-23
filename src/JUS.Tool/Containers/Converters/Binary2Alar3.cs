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
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Containers.Converters
{
    /// <summary>
    /// Converts between a binary format and <see cref="Alar3"/>.
    /// </summary>
    public class Binary2Alar3 : IConverter<IBinary, Alar3>
    {
        private DataReader reader = null!;
        private Alar3 alar = null!;

        /// <summary>
        /// Converts BinaryFormat to Alar3.
        /// </summary>
        /// <param name="source">File to transform.</param>
        /// <returns>Alar3.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public Alar3 Convert(IBinary source)
        {
            ArgumentNullException.ThrowIfNull(source);

            reader = new DataReader(source.Stream);
            reader.Stream.Position = 0;

            int numFiles = ReadHeader();

            for (int i = 0; i < numFiles; i++) {
                ushort offset = reader.ReadUInt16();
                source.Stream.RunInPosition(() => ReadFileInfo(source), offset);
            }

            return alar;
        }

        private int ReadHeader()
        {
            string stamp = reader.ReadString(4);
            if (stamp != Alar3.STAMP) {
                throw new FormatException("Invalid header");
            }

            if (reader.ReadByte() != 3) {
                throw new FormatException("Invalid format version");
            }

            byte featureFlags = reader.ReadByte();
            if (!Alar3.SupportedFeatureFlags.Contains(featureFlags)) {
                throw new FormatException($"Unsupported feature flags: {featureFlags:X}");
            }

            ushort numFiles = reader.ReadUInt16();
            uint firstFileId = reader.ReadUInt32();
            uint lastFileId = reader.ReadUInt32();
            _ = reader.ReadUInt16(); // offset to file data

            alar = new Alar3 {
                FeatureFlags = featureFlags,
                FirstFileId = firstFileId,
                LastFileId = lastFileId,
            };

            return numFiles;
        }

        private void ReadFileInfo(IBinary source)
        {
            uint id = reader.ReadUInt32();
            uint offset = reader.ReadUInt32();
            uint size = reader.ReadUInt32();
            uint flags = reader.ReadUInt32();

            var fileStream = new DataStream(source.Stream, offset, size);
            var alarFile = new Alar3File(fileStream) {
                FileId = id,
                Flags = flags,
            };

            string dir = string.Empty;
            Node child;
            if ((flags >> 31) == 1) {
                _ = reader.ReadUInt16(); // name hash
                string path = reader.ReadString();

                string name = Path.GetFileName(path);
                dir = Path.GetDirectoryName(path)!;
                child = new Node(name, alarFile);
            } else {
                child = new Node("file", alarFile);
            }

            NodeFactory.CreateContainersForChild(alar.Root, dir, child);
        }
    }
}
