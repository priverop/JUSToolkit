// Copyright (c) 2022 Priverop
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
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Containers.Converters
{
    /// <summary>
    /// Converts a binary format with ALAR v2 data into a container.
    /// </summary>
    public class Binary2Alar2 : IConverter<IBinary, Alar>
    {
        private DataReader reader = null!;
        private Alar alar = null!;

        /// <summary>
        /// Converts a BinaryFormat to an Alar2 container.
        /// </summary>
        /// <param name="input">IBinary node.</param>
        /// <returns>Alar2 NodeContainerFormat.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> is <c>null</c>.</exception>
        public Alar Convert(IBinary input)
        {
            ArgumentNullException.ThrowIfNull(input);

            reader = new DataReader(input.Stream);
            reader.Stream.Position = 0;

            alar = new Alar();
            (AlarInfo info, int numFiles) = ReadHeader();

            for (int i = 0; i < numFiles; i++) {
                uint fileId = reader.ReadUInt32();
                uint offset = reader.ReadUInt32();
                uint size = reader.ReadUInt32();
                uint flags = reader.ReadUInt32();

                var fileStream = new DataStream(input.Stream, offset, size);
                var alarFile = new AlarFileInfo {
                    FileId = fileId,
                    Flags = flags,
                };

                info.FilesMetadata.Add(alarFile);
                AppendChild(alarFile, new BinaryFormat(fileStream), offset);
            }

            return alar;
        }

        private (AlarInfo, int) ReadHeader()
        {
            string stamp = reader.ReadString(4);
            if (stamp != Alar.FormatId) {
                throw new FormatException("Invalid header");
            }

            if (reader.ReadByte() != 2) {
                throw new FormatException("Invalid format version");
            }

            var featureFlags = (AlarFormatFeatures)reader.ReadByte();
            ushort fileCount = reader.ReadUInt16();
            uint firstFileId = reader.ReadUInt32();
            uint lastFileId = reader.ReadUInt32();

            var alarInfo = new AlarInfo {
                Version = 2,
                Features = featureFlags,
                FirstFileId = firstFileId,
                LastFileId = lastFileId,
            };
            alar.Root.Add(new Node(Alar.InfoNodeName, alarInfo));

            return (alarInfo, fileCount);
        }

        private void AppendChild(AlarFileInfo alarFile, BinaryFormat dataStream, uint dataOffset)
        {
            string filename = "file";

            bool hasFilename = (alarFile.Flags >> 31) == 1;
            if (hasFilename) {
                using (reader.Stream.EnterWithPosition(dataOffset - 0x22)) {
                    filename = reader.ReadString();
                }
            }

            alarFile.Path = filename;

            var child = new Node(filename, dataStream);
            alar.Root.Add(child);
        }
    }
}
