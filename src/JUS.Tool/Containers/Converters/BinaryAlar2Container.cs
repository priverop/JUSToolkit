// Copyright (c) 2021 SceneGate

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
namespace Texim.Games.JumpUltimateStars
{
    using System;
    using System.IO;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    public class BinaryAlar2Container : IConverter<IBinary, NodeContainerFormat>
    {
        private const string STAMP = "ALAR";
        private static readonly Version SupportedVersion = new Version(3, 5);

        private DataReader reader;
        private NodeContainerFormat container;

        private int numEntries;

        public NodeContainerFormat Convert(IBinary source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            reader = new DataReader(source.Stream);
            container = new NodeContainerFormat();

            ReadHeader();

            for (int i = 0; i < numEntries; i++) {
                ushort infoOffset = reader.ReadUInt16();
                source.Stream.RunInPosition(() => ReadFileInfo(), infoOffset);
            }

            return container;
        }

        private void ReadHeader()
        {
            string stamp = reader.ReadString(4);
            if (stamp != STAMP) {
                throw new FormatException("Invalid header");
            }

            var version = new Version(reader.ReadByte(), reader.ReadByte());
            if (version != SupportedVersion) {
                throw new FormatException($"Unsupported version: {version:X}");
            }

            reader.Stream.Position += 6; // numFiles + reserved
            numEntries = reader.ReadInt32();
            reader.Stream.Position += 2; // data offset
        }

        private void ReadFileInfo()
        {
            short id = reader.ReadInt16();
            short unknown = reader.ReadInt16();
            long offset = reader.ReadInt32();
            int size = reader.ReadInt32();
            short unknown2 = reader.ReadInt16();
            short unknown3 = reader.ReadInt16();
            short unknown4 = reader.ReadInt16();
            string path = reader.ReadString();

            string name = Path.GetFileName(path);
            string dir = Path.GetDirectoryName(path);
            Node child = NodeFactory.FromSubstream(name, reader.Stream, offset, size);
            child.Tags["JUS_ID"] = id;
            child.Tags["JUS_UNK1"] = unknown;
            child.Tags["JUS_UNK2"] = unknown2;
            child.Tags["JUS_UNK3"] = unknown3;
            child.Tags["JUS_UNK4"] = unknown4;

            NodeFactory.CreateContainersForChild(container.Root, dir, child);
        }
    }
}
