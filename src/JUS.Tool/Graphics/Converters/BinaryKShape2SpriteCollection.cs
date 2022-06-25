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
using Yarhl.IO;

namespace Texim.Games.JumpUltimateStars
{
    /// <summary>
    /// Converts between a <see cref="KShapeSprites"/> and a Binary File.
    /// </summary>
    public class BinaryKShape2SpriteCollection : IConverter<IBinary, KShapeSprites>
    {
        internal const int NumGroups = 8;
        internal const int GroupSizeOffset = NumGroups * 4; // after offset table, each offset 4 bytes
        internal const int DataOffset = (NumGroups * 2) * 4;

        internal const int EntrySize = 0x18;
        internal const int SpriteInfoSize = 0x14; // width * height / (blocksize * blocksize)

        private const int Width = 240;
        private const int Height = 192;
        private const int SegmentDimensions = 48;
        private const int TilesPerSegment = SegmentDimensions * SegmentDimensions / 64; // tiles of 8x8

        /// <summary>
        /// Converts a BinaryFile into a KShapeSprites collection.
        /// </summary>
        /// <param name="source">BinaryFile node.</param>
        /// <returns>KShapeSprites Node.</returns>
        public KShapeSprites Convert(IBinary source)
        {
            if (source is null) {
                throw new ArgumentNullException(nameof(source));
            }

            var reader = new DataReader(source.Stream);
            var kshape = new KShapeSprites();

            for (int i = 0; i < NumGroups; i++) {
                source.Stream.Position = i * 4;
                int firstElement = reader.ReadInt32();

                source.Stream.Position = GroupSizeOffset + (i * 4);
                int numElements = reader.ReadInt32();

                for (int e = 0; e < numElements; e++) {
                    source.Stream.Position = DataOffset + ((firstElement + e) * EntrySize);
                    var sprite = ReadSprite(reader);
                    kshape.AddSprite(i, e, sprite);
                }
            }

            return kshape;
        }

        private static Sprite ReadSprite(DataReader reader)
        {
            byte[] info = reader.ReadBytes(SpriteInfoSize);

            var sprite = new Sprite {
                Width = Width,
                Height = Height,
            };

            int x = 0;
            int y = 0;
            for (int i = 0; i < info.Length; i++) {
                // If the segment index is 0, then it's transparent, skip.
                if (info[i] > 0) {
                    int segmentIdx = info[i] - 1;
                    int tileIdx = (segmentIdx * TilesPerSegment) + 1; // skip first transparent tile
                    var segment = new ImageSegment {
                        Width = SegmentDimensions,
                        Height = SegmentDimensions,
                        TileIndex = tileIdx,
                        CoordinateX = x,
                        CoordinateY = y,
                    };
                    sprite.Segments.Add(segment);
                }

                x += SegmentDimensions;
                if (x == Width) {
                    x = 0;
                    y += SegmentDimensions;
                }
            }

            return sprite;
        }
    }
}