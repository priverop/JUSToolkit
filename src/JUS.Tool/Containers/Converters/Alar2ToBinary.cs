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
    /// Converts an ALAR container into a binary ALAR v2 format.
    /// </summary>
    public class Alar2ToBinary : IConverter<Alar, BinaryFormat>
    {
        private DataWriter writer = null!;

        /// <inheritdoc/>
        public BinaryFormat Convert(Alar alar)
        {
            ArgumentNullException.ThrowIfNull(alar);

            var binary = new BinaryFormat();
            writer = new DataWriter(binary.Stream);

            WriteHeader(alar);

            // Pre-fill the file info table so we can write the file data (and know the offset)
            int fileInfoTableLength = alar.Root.Children.Count * 0x10;
            writer.WriteTimes(0x00, fileInfoTableLength);

            writer.Stream.Position = 0x10;
            foreach (Node child in alar.Root.Children) {
                WriteFile(child);
            }

            return binary;
        }

        private void WriteHeader(Alar alar)
        {
            writer.Write(Alar.FormatId, false);
            writer.Write(alar.Version);
            writer.Write((byte)alar.Features);
            writer.Write((ushort)alar.Root.Children.Count);
            writer.Write(alar.FirstFileId);
            writer.Write(alar.LastFileId);
        }

        private void WriteFile(Node child)
        {
            AlarFile fileInfo = child.GetFormatAs<AlarFile>() ?? throw new FormatException("Invalid format");
            bool hasFilename = (fileInfo.Flags >> 31) == 1;

            uint nameLength = hasFilename ? 0x24u : 0x00;
            uint fileOffset = (uint)writer.Stream.Length.Pad(4) + nameLength;

            writer.Write(fileInfo.FileId);
            writer.Write(fileOffset);
            writer.Write((uint)child.Stream!.Length);
            writer.Write(fileInfo.Flags);

            writer.Stream.PushToPosition(0, SeekOrigin.End);
            writer.WritePadding(0x00, 4);

            if (hasFilename) {
                ushort nameHash = AlarPathHash.ComputeV1(child.Name);

                writer.Write((ushort)0x00); // padding
                writer.Write(child.Name, 0x20);
                writer.Write(nameHash);
            }

            child.Stream.WriteTo(writer.Stream);
            writer.Stream.PopPosition();
        }
    }
}
