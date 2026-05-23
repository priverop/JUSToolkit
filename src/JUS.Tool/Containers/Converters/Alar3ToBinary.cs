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
    /// Converts between a NodeContainerFormat and a BinaryFormat file.
    /// </summary>
    public class Alar3ToBinary :
    IConverter<Alar3, BinaryFormat>
    {
        /// <summary>
        /// Converts Alar3 to BinaryFormat.
        /// </summary>
        /// <param name="alar">Alar3 NodeContainerFormat.</param>
        /// <returns>BinaryFormat Node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="alar"/> is <c>null</c>.</exception>
        public BinaryFormat Convert(Alar3 alar)
        {
            ArgumentNullException.ThrowIfNull(alar);

            // Iterate in the expected order and pre-compute all container paths,
            // so we can calculate the section lengths.
            FileEntry[] entries = GetContainerEntries(alar.Root);

            int fileInfoTableOffset = 0x12;
            int fileInfoTableLength = entries.Length * 2;

            int fileInfoSectionOffset = (fileInfoTableOffset + fileInfoTableLength).Pad(4);
            int fileInfoSectionLength = entries.Sum(e => e.EncodedInfoLength);
            int fileDataSectionOffset = fileInfoSectionOffset + fileInfoSectionLength;

            var binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream);

            // Write the header
            writer.Write(Alar3.STAMP, false);
            writer.Write((byte)3);
            writer.Write(alar.FeatureFlags);
            writer.Write((ushort)entries.Length);
            writer.Write(alar.FirstFileId);
            writer.Write(alar.LastFileId);
            writer.Write((ushort)fileDataSectionOffset);

            // Pre-fill info table and info section so we can write everything at the same time
            writer.WriteUntilLength(0x00, fileDataSectionOffset);

            // We leave the writer position at the file info section as it's variable.
            // The other sections (file info table, file data) are easy to calculate.
            writer.Stream.Position = fileInfoSectionOffset;
            for (int fileIdx = 0; fileIdx < entries.Length; fileIdx++) {
                FileEntry entry = entries[fileIdx];

                ushort fileInfoOffset = (ushort)writer.Stream.Position;
                uint fileDataOffset = (uint)writer.Stream.Length.Pad(4); // padding written later, except last file

                // Write file info offset
                writer.Stream.PushToPosition(fileInfoTableOffset + (fileIdx * 2));
                writer.Write(fileInfoOffset);
                writer.Stream.PopPosition();

                // Write file info
                writer.Write(entry.FileInfo.FileID);
                writer.Write(fileDataOffset);
                writer.Write((uint)entry.Data.Length);
                writer.Write(entry.FileInfo.Flags);
                writer.Write(entry.FileInfo.FilenameHash);
                writer.Write(entry.ContainerPath);
                writer.WritePadding(0, 4);

                // Write file data
                writer.Stream.PushToPosition(0, SeekOrigin.End);
                writer.WritePadding(0, 4);
                entry.Data.WriteTo(writer.Stream);
                writer.Stream.PopPosition();
            }

            return binary;
        }

        private static FileEntry[] GetContainerEntries(Node root)
        {
            List<FileEntry> entries = [];
            foreach (Node node in Navigator.IterateNodes(root, NavigationMode.DepthFirst)) {
                if (node.IsContainer) {
                    continue;
                }

                string containerPath = Path.GetRelativePath(root.Path, node.Path).Replace('\\', '/');
                Alar3File fileInfo = node.GetFormatAs<Alar3File>()
                    ?? throw new FormatException($"Unexpected file format for {node.Path}");
                entries.Add(new FileEntry(node.Stream!, fileInfo, containerPath));
            }

            return entries.ToArray();
        }

        private sealed record FileEntry(DataStream Data, Alar3File FileInfo, string ContainerPath)
        {
            /// <summary>
            /// Gets the binary encoded length of the path, assuming ASCII characters and a null-terminator.
            /// </summary>
            public int EncodedPathLength => ContainerPath.Length + 1;

            public int EncodedInfoLength => (0x12 + EncodedPathLength).Pad(4);
        }
    }
}
