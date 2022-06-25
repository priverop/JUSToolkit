// Copyright (c) 2022 SceneGate

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
using Texim.Sprites;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace Texim.Games.JumpUltimateStars {
    /// <summary>
    /// Converts between a BinaryFormat (a file) containing a Dstx Format and a SpriteImage.
    /// </summary>
    public class BinaryDstx2SpriteImage : IConverter<IBinary, NodeContainerFormat>
    {
        private const string Stamp = "DSTX";
        private const int Type = 0x04;

        private readonly BinaryDsig2IndexedPaletteImage dsigConverter = new ();

        /// <summary>
        /// Converts a BinaryFormat (file) to a NodeContainerFormat.
        /// </summary>
        /// <param name="source">File to convert.</param>
        /// <returns>NodeContainerFormat Node.</returns>
        public NodeContainerFormat Convert(IBinary source)
        {
            if (source is null) {
                throw new ArgumentNullException(nameof(source));
            }

            source.Stream.Position = 0;
            var reader = new DataReader(source.Stream);

            // Read header
            string stamp = reader.ReadString(4);
            if (stamp != Stamp) {
                throw new FormatException($"Invalid stamp '{stamp}'");
            }

            _ = reader.ReadByte(); // unknown
            byte type = reader.ReadByte();
            if (type != Type) {
                throw new FormatException($"Invalid type: 0x{type:X2}");
            }

            int numSegments = reader.ReadInt16();
            int dsigOffset = reader.ReadInt16();
            _ = reader.ReadInt16(); // unknown

            Sprite sprite = ReadSprite(reader, numSegments);

            using var dsigBinary = new BinaryFormat(
                source.Stream,
                dsigOffset,
                source.Stream.Length - dsigOffset);
            Images.IndexedPaletteImage image = dsigConverter.Convert(dsigBinary);

            var container = new NodeContainerFormat();
            container.Root.Add(new Node("sprite", sprite));
            container.Root.Add(new Node("image", image));

            return container;
        }

        private static Sprite ReadSprite(DataReader reader, int numSegments)
        {
            int y = 0;
            var sprite = new Sprite();
            for (int i = 0; i < numSegments; i++) {
                int width = reader.ReadByte();
                int height = reader.ReadByte();
                short tileIndex = reader.ReadInt16();
                var segment = new ImageSegment {
                    Width = width * 8,
                    Height = height * 8,
                    TileIndex = (tileIndex == 0) ? 1 : tileIndex,
                    CoordinateX = 0,
                    CoordinateY = y,
                };

                y += segment.Height;
                sprite.Segments.Add(segment);
            }

            sprite.Width = 48;
            sprite.Height = y;
            return sprite;
        }
    }
}