using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using SixLabors.ImageSharp.Processing;
using Texim.Images;
using Texim.Sprites;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Graphics.Converters
{
    public class BinaryToDtx3 : IConverter<IBinary, NodeContainerFormat>
    {
        private const string Stamp = "DSTX";
        private const int Version = 0x01;
        private const int Type = 0x03;
        private const int PointerOffset = 0x0A;

        private readonly Binary2Dig digConverter = new ();

        public NodeContainerFormat Convert(IBinary source)
        {
            if (source is null) {
                throw new ArgumentNullException(nameof(source));
            }

            var reader = new DataReader(source.Stream);
            source.Stream.Position = 0;

            if (reader.ReadString(4) != Stamp) {
                throw new FormatException("Invalid stamp");
            }

            int version = reader.ReadByte();
            int type = reader.ReadByte();

            if (version != Version && type != Type) {
                throw new FormatException($"Invalid version: {version}.{type}");
            }

            ushort numSprites = reader.ReadUInt16();
            ushort size = reader.ReadUInt16();
            var sprites = new NodeContainerFormat();

            for (int i = 0; i < numSprites; i++) {
                sprites.Root.Add(new Node($"sp_{i:00}", ReadSprite(reader)));
            }

            reader.SkipPadding(0x04);

            using var dsigBinary = new BinaryFormat(
                source.Stream,
                size,
                source.Stream.Length - size);
            IndexedPaletteImage image = digConverter.Convert(dsigBinary);

            var container = new NodeContainerFormat();
            container.Root.Add(new Node("sprites", sprites));
            container.Root.Add(new Node("image", image));

            return container;
        }

        private Sprite ReadSprite(DataReader reader)
        {
            int spriteOffset = reader.ReadUInt16() + PointerOffset;
            reader.Stream.PushToPosition(spriteOffset);

            var sprite = new Sprite();
            ushort numSegments = reader.ReadUInt16();

            for (int i = 0; i < numSegments; i++) {
                ushort tileIndex = reader.ReadUInt16();
                sbyte xPos = reader.ReadSByte();
                sbyte yPos = reader.ReadSByte();
                byte shape = reader.ReadByte();
                byte paletteIndex = reader.ReadByte();
                var (width, height) = GetSize(shape);

                var segment = new ImageSegment() {
                    TileIndex = tileIndex,
                    CoordinateX = xPos,
                    CoordinateY = yPos,
                    PaletteIndex = paletteIndex,
                    Width = width,
                    Height = height,
                };

                sprite.Segments.Add(segment);
            }

            sprite.Width = 256;
            sprite.Height = 256;
            reader.Stream.PopPosition();

            return sprite;
        }

        private (int width, int height) GetSize(byte shape)
        {
            int[] sizes = new int[4] { 8, 16, 32, 64 };

            return shape switch {
                0 => (sizes[0], sizes[0]),
                1 => (sizes[1], sizes[1]),
                2 => (sizes[2], sizes[2]),
                3 => (sizes[3], sizes[3]), // ???
                4 => (sizes[1], sizes[0]),
                5 => (sizes[2], sizes[0]),
                6 => (sizes[2], sizes[1]),
                7 => (sizes[3], sizes[2]),
                8 => (sizes[0], sizes[1]),
                9 => (sizes[0], sizes[2]),
                10 => (sizes[1], sizes[2]),
                11 => (sizes[1], sizes[3]), // ???
                19 => (sizes[0], sizes[0]), // ???
                23 => (sizes[0], sizes[0]), // ???
                35 => (sizes[0], sizes[0]), // ???
                51 => (sizes[0], sizes[0]), // ???
                _ => throw new FormatException($"Unknown shape: {shape}")
            };
        }
    }
}
