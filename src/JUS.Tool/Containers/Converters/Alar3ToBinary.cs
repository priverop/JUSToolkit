// Copyright (c) 2022 Pablo Rivero

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
            if (alar == null) {
                throw new ArgumentNullException(nameof(alar));
            }

            var binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream) {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEncoding("ascii"),
            };

            // Write Header
            writer.Write(Alar3.STAMP, false);
            writer.Write((byte)3);
            writer.Write(alar.MinorVersion);
            writer.Write(alar.NumFiles);
            writer.Write(alar.Reserved);
            writer.Write(alar.NumEntries);
            writer.Write(alar.DataOffset);

            // Write File Pointers Section
            for (ushort i = 0; i < alar.NumFiles; i++) {
                writer.Write(alar.FileInfoPointers[i]);
            }

            // We store the positions of the alar file Offset section
            // so we can modify them later.
            long[] offsetPositions = new long[alar.NumFiles];

            // As every file has a random padding, we store it so we can write it
            // later.
            int[] paddings = new int[alar.NumFiles];

            // Write File Info Section
            foreach (Node alarFile in Navigator.IterateNodes(alar.Root)) {
                if (!alarFile.IsContainer) {
                    Alar3File alarChild = alarFile.GetFormatAs<Alar3File>();

                    writer.WritePadding(0, 04);

                    writer.Write(alarChild.FileID);
                    writer.Write(alarChild.Unknown);
                    offsetPositions[alarChild.FileID] = writer.Stream.Position;
                    writer.Write(alarChild.Offset);
                    writer.Write(alarChild.Size);
                    writer.Write(alarChild.Unknown2);
                    writer.Write(alarChild.Unknown3);
                    writer.Write(alarChild.Unknown4);

                    writer.Write(GetAlar3Path(alarFile.Path, alar.Root.Name), true);
                }
            }

            writer.WritePadding(0, 04);

            // Write File Data Section
            foreach (Node node in Navigator.IterateNodes(alar.Root)) {
                if (!node.IsContainer) {
                    Alar3File alarFile = node.GetFormatAs<Alar3File>();

                    // Storing Padding for every file but the first one
                    if (alarFile.FileID != 0) {
                        long initPadding = writer.Stream.Position;
                        writer.WritePadding(0, 04);
                        long endPadding = writer.Stream.Position;
                        int paddingSize = (int)(endPadding - initPadding);

                        paddings[alarFile.FileID] = paddingSize;
                    }

                    alarFile.Stream.WriteTo(writer.Stream);
                }
            }

            // Rewrite Offsets
            int newOffset = 0;
            foreach (Node node in Navigator.IterateNodes(alar.Root)) {
                if (!node.IsContainer) {
                    Alar3File alarFile = node.GetFormatAs<Alar3File>();

                    // Starter Offset
                    if (alarFile.FileID == 0) {
                        newOffset = (int)alarFile.Offset;
                    }

                    // Modify the size of the file
                    writer.Stream.RunInPosition(
                            () => writer.Write(alarFile.Size),
                            offsetPositions[alarFile.FileID] + 4); // The size is always 4 positions ahead

                    // Add the size of the file and the padding
                    if (alarFile.FileID != alar.NumFiles - 1) {
                        newOffset += (int)(alarFile.Size + paddings[alarFile.FileID + 1]);
                        writer.Stream.RunInPosition(
                            () => writer.Write(newOffset),
                            offsetPositions[alarFile.FileID + 1]);
                    }
                }
            }

            return binary;
        }

        /// <summary>
        /// Removes the alar filename (the root name) from the path of the node.
        /// <remarks>If we have '/alar.alar/komas/dg_00.dtx' we will get 'komas/dg_00.dtx'.</remarks>
        /// </summary>
        /// <param name="fullPath">The full path of the node.</param>
        /// <param name="alarName">The name of the root node.</param>
        /// <returns>The string.</returns>
        private string GetAlar3Path(string fullPath, string alarName)
        {
            return fullPath.Substring(1).Replace(alarName, string.Empty).Substring(1);
        }
    }
}
