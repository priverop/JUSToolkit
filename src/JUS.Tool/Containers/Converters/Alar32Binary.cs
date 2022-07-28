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
using System.Linq;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Containers.Converters
{
    /// <summary>
    /// Converts between a NodeContainerFormat and a BinaryFormat file.
    /// </summary>
    public class Alar32Binary :
    IConverter<Alar3, BinaryFormat>
    {
        /// <summary>
        /// Converts Alar3 to BinaryFormat.
        /// </summary>
        /// <param name="aar">Alar3 NodeContainerFormat.</param>
        /// <returns>BinaryFormat Node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="aar"/> is <c>null</c>.</exception>
        public BinaryFormat Convert(Alar3 aar)
        {
            if (aar == null) {
                throw new ArgumentNullException(nameof(aar));
            }

            var binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream) {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEncoding("ascii"),
            };

            writer.Write(Alar3.STAMP, false);
            writer.Write((byte)Alar3.SupportedVersion.Major);
            writer.Write((byte)Alar3.SupportedVersion.Minor);
            writer.Write(aar.NumFiles);
            writer.Write(aar.Reserved);
            writer.Write(aar.NumEntries);
            writer.Write(aar.DataOffset);

            for (ushort i = 0; i < aar.NumFiles; i++) {
                writer.Write(aar.FileInfoPointers[i]);
            }

            long[] offsetPositions = new long[aar.NumFiles];
            int[] paddings = new int[aar.NumFiles];

            foreach (Node aarFile in Navigator.IterateNodes(aar.Root)) {
                if (!aarFile.IsContainer) {
                    Alar3File alarChild = aarFile.GetFormatAs<Alar3File>();

                    writer.WritePadding(0, 04);

                    writer.Write(alarChild.FileID);
                    writer.Write(alarChild.Unknown);
                    offsetPositions[alarChild.FileID] = writer.Stream.Position;
                    writer.Write(alarChild.Offset);
                    writer.Write(alarChild.Size);
                    writer.Write(alarChild.Unknown2);
                    writer.Write(alarChild.Unknown3);
                    writer.Write(alarChild.Unknown4);

                    writer.Write(GetAlar3Path(aarFile.Path, aar.Root.Name), true);
                }
            }

            writer.WritePadding(0, 04);

            // Primero Cabeceras y luego ficheros
            // TODO: AQUI HAY ALGUN ERROR, ficheros de más?
            foreach (Node node in Navigator.IterateNodes(aar.Root)) {
                if (!node.IsContainer) {
                    Alar3File aarFile = node.GetFormatAs<Alar3File>();

                    if (aarFile.FileID != 0) {
                        long initPadding = writer.Stream.Position;
                        writer.WritePadding(0, 04);
                        long endPadding = writer.Stream.Position;
                        int paddingSize = (int)(endPadding - initPadding);

                        paddings[aarFile.FileID] = paddingSize;
                    }

                    aarFile.Stream.WriteTo(writer.Stream);
                }
            }

            int newOffset = 0;
            foreach (Node node in Navigator.IterateNodes(aar.Root)) {
                if (!node.IsContainer) {
                    Alar3File aarFile = node.GetFormatAs<Alar3File>();

                    if (aarFile.FileID == 0) {
                        newOffset = (int)aarFile.Offset;
                    }

                    if (aarFile.FileID != aar.NumFiles - 1) {
                        newOffset += (int)(aarFile.Size + paddings[aarFile.FileID + 1]);
                        writer.Stream.RunInPosition(
                            () => writer.Write(newOffset),
                            offsetPositions[aarFile.FileID + 1]);
                    }
                }
            }

            return binary;
        }

        private string GetAlar3Path(string fullPath, string alarName)
        {
            return fullPath.Substring(1).Replace(alarName, string.Empty).Substring(1);
        }
    }
}
