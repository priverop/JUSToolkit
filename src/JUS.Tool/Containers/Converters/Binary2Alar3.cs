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
    /// Converts a binary format with ALAR v2 data into a container.
    /// </summary>
    public class Binary2Alar3 : IConverter<IBinary, Alar>
    {
        private DataReader reader = null!;
        private Alar alar = null!;

        /// <summary>
        /// Converts BinaryFormat to Alar3.
        /// </summary>
        /// <param name="source">File to transform.</param>
        /// <returns>Alar3.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        public Alar Convert(IBinary source)
        {
            ArgumentNullException.ThrowIfNull(source);

            reader = new DataReader(source.Stream);
            reader.Stream.Position = 0;

            (AlarInfo info, int numFiles) = ReadHeader();

            for (int i = 0; i < numFiles; i++) {
                ushort offset = reader.ReadUInt16();
                source.Stream.RunInPosition(() => ReadFileInfo(source, info), offset);
            }

            return alar;
        }

        private (AlarInfo, int) ReadHeader()
        {
            string stamp = reader.ReadString(4);
            if (stamp != Alar.FormatId) {
                throw new FormatException("Invalid header");
            }

            if (reader.ReadByte() != 3) {
                throw new FormatException("Invalid format version");
            }

            var featureFlags = (AlarFormatFeatures)reader.ReadByte();
            ushort numFiles = reader.ReadUInt16();
            uint firstFileId = reader.ReadUInt32();
            uint lastFileId = reader.ReadUInt32();
            _ = reader.ReadUInt16(); // offset to file data

            alar = new Alar();
            var alarInfo = new AlarInfo {
                Version = 3,
                Features = featureFlags,
                FirstFileId = firstFileId,
                LastFileId = lastFileId,
            };
            alar.Root.Add(new Node(Alar.InfoNodeName, alarInfo));

            return (alarInfo, numFiles);
        }

        private void ReadFileInfo(IBinary source, AlarInfo info)
        {
            uint id = reader.ReadUInt32();
            uint offset = reader.ReadUInt32();
            uint size = reader.ReadUInt32();
            uint flags = reader.ReadUInt32();

            var binaryFile = new BinaryFormat(source.Stream, offset, size);
            var alarFileInfo = new AlarFileInfo {
                FileId = id,
                Flags = flags,
            };

            string dir = string.Empty;
            Node child;
            if ((flags >> 31) == 1) {
                _ = reader.ReadUInt16(); // name hash
                string path = reader.ReadString();
                alarFileInfo.Path = path;

                string name = Path.GetFileName(path);
                dir = Path.GetDirectoryName(path)!;
                child = new Node(name, binaryFile);
            } else {
                child = new Node("file", binaryFile);
            }

            info.FilesMetadata.Add(alarFileInfo);
            NodeFactory.CreateContainersForChild(alar.Root, dir, child);
        }
    }
}
