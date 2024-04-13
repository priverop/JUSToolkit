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
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Containers.Converters
{
    /// <summary>
    /// Converts between a NodeContainerFormat and a BinaryFormat file.
    /// </summary>
    public class Alar2ToBinary : IConverter<Alar2, BinaryFormat>
    {
        private DataWriter writer;

        /// <summary>
        /// Converts Alar2 to BinaryFormat.
        /// </summary>
        /// <param name="alar">Alar2 NodeContainerFormat.</param>
        /// <returns>BinaryFormat Node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="alar"/> is <c>null</c>.</exception>
        public BinaryFormat Convert(Alar2 alar)
        {
            if (alar == null) {
                throw new ArgumentNullException(nameof(alar));
            }

            var binary = new BinaryFormat();
            writer = new DataWriter(binary.Stream);

            WriteHeader(alar);
            WriteFileInfoSection(alar);
            WriteFileDataSection(alar);
            RewriteOffsets(alar);

            return binary;
        }

        private void WriteHeader(Alar2 alar)
        {
            writer.Write(Alar2.STAMP, false);
            writer.Write((byte)Alar2.SupportedVersion.Major);
            writer.Write((byte)Alar2.SupportedVersion.Minor);
            writer.Write(alar.NumFiles);
            writer.Write(alar.IDs);
        }

        private void WriteFileInfoSection(Alar2 alar)
        {
            foreach (Node alarFile in Navigator.IterateNodes(alar.Root)) {
                if (!alarFile.IsContainer) {
                    Alar2File alarChild = alarFile.GetFormatAs<Alar2File>();

                    writer.Write(alarChild.FileID);
                    writer.Write(alarChild.Offset);
                    writer.Write(alarChild.Size);
                    writer.Write(alarChild.Unknown);
                }
            }
        }

        private void WriteFileDataSection(Alar2 alar)
        {
            foreach (Node alarFile in Navigator.IterateNodes(alar.Root)) {
                if (!alarFile.IsContainer) {
                    writer.WriteTimes(0, 2);
                    writer.Write(alarFile.Name);

                    // Tenemos que pintar ceros hasta el offset del puntero - 02 (que hay un unknown)
                    int times = 32 - alarFile.Name.Length - 1; // 1 null byte
                    writer.WriteTimes(0, times);

                    Alar2File alarChild = alarFile.GetFormatAs<Alar2File>();

                    writer.Write(alarChild.Unknown2);
                    alarChild.Stream.WriteTo(writer.Stream);
                }
            }
        }

        private void RewriteOffsets(Alar2 alar)
        {
            int offsetPostion = 0x04; // The first file position
            int sizePostion = 0x08; // The first file position
            int newOffset = 0;
            foreach (Node node in Navigator.IterateNodes(alar.Root)) {
                if (!node.IsContainer) {
                    Alar2File alarFile = node.GetFormatAs<Alar2File>();

                    // Starter Offset
                    if (alarFile.FileNum == 1) {
                        newOffset = (int)alarFile.Offset;
                    }

                    // Modify the size of the file
                    writer.Stream.RunInPosition(
                            () => writer.Write(alarFile.Size),
                            sizePostion + (0x10 * alarFile.FileNum));

                    // Add the size of the file to get the next file offset
                    if (alarFile.FileNum != alar.NumFiles) {
                        newOffset += (int)(alarFile.Size + 36); // 36 bytes from the file info section
                        writer.Stream.RunInPosition(
                            () => writer.Write(newOffset),
                            offsetPostion + (0x10 * (alarFile.FileNum + 1))); // next file
                    }
                }
            }
        }

        /// <summary>
        /// Removes the alar filename (the root name) from the path of the node.
        /// <remarks>If we have '/alar.alar/komas/dg_00.dtx' we will get 'komas/dg_00.dtx'.</remarks>
        /// </summary>
        /// <param name="fullPath">The full path of the node.</param>
        /// <param name="alarName">The name of the root node.</param>
        /// <returns>The string.</returns>
        private string GetAlar2Path(string fullPath, string alarName)
        {
            return fullPath.Substring(1).Replace(alarName, string.Empty).Substring(1);
        }
    }
}
