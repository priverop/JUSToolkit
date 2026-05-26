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
    /// Converts an ALAR container into a binary ALAR v3 format.
    /// </summary>
    public class Alar3ToBinary :
    IConverter<Alar, BinaryFormat>
    {
        /// <inheritdoc/>
        public BinaryFormat Convert(Alar alar)
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

            bool useHashV1 = !alar.Features.HasFlag(AlarFormatFeatures.PathHashV2);

            var binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream);

            // Write the header
            writer.Write(Alar.FormatId, false);
            writer.Write(alar.Version);
            writer.Write((byte)alar.Features);
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
                ushort nameHash = useHashV1 ? AlarPathHash.ComputeV1(entry.ContainerPath) : AlarPathHash.ComputeV2(entry.ContainerPath);
                writer.Write(entry.FileInfo.FileId);
                writer.Write(fileDataOffset);
                writer.Write((uint)entry.Data.Length);
                writer.Write(entry.FileInfo.Flags);
                writer.Write(nameHash);
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

        internal static string GetRelativeChildPath(string rootPath, string childPath)
        {
            return Path.GetRelativePath(rootPath, childPath).Replace('\\', '/');
        }

        private static FileEntry[] GetContainerEntries(Node root)
        {
            List<FileEntry> entries = [];
            foreach (Node node in Navigator.IterateNodes(root, NavigationMode.DepthFirst)) {
                if (node.IsContainer) {
                    continue;
                }

                string containerPath = GetRelativeChildPath(root.Path, node.Path);
                AlarFile fileInfo = node.GetFormatAs<AlarFile>()
                    ?? throw new FormatException($"Unexpected file format for {node.Path}");
                entries.Add(new FileEntry(node.Stream!, fileInfo, containerPath));
            }

            return entries.ToArray();
        }


        private sealed record FileEntry(DataStream Data, AlarFile FileInfo, string ContainerPath)
        {
            /// <summary>
            /// Gets the binary encoded length of the path, assuming ASCII characters and a null-terminator.
            /// </summary>
            public int EncodedPathLength => ContainerPath.Length + 1;

            public int EncodedInfoLength => (0x12 + EncodedPathLength).Pad(4);
        }
    }
}
