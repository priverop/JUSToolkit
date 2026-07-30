using System;
using JUS.Tool.Graphics;
using JUS.Tool.Graphics.Converters;
using Texim.Pixels;
using Texim.Sprites;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Graphics.Converters
{
    /// <summary>
    /// Converts between a .dtx 3 file and a sprite collection and a <see cref="Dig"/> image.
    /// </summary>
    public class BinaryToDtx3 : IConverter<IBinary, NodeContainerFormat>
    {
        private const string Stamp = "DSTX";
        private const int Version = 0x01;
        private const int Type = 0x03;
        private const int PointerOffset = 0x0A;
        private readonly Binary2Dig digConverter = new();
        private List<Sprite> spriteCollection = [];

        /// <summary>
        /// Converts a <see cref="BinaryFormat"/> (file) to a dtx3 <see cref="NodeContainerFormat"/>.
        /// </summary>
        /// <param name="source">File to convert.</param>
        /// <returns><see cref="NodeContainerFormat"/> with all the sprites and <see cref="Dig"/> image.</returns>
        public NodeContainerFormat Convert(IBinary source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var reader = new DataReader(source.Stream);
            source.Stream.Position = 0;

            if (reader.ReadString(4) != Stamp) {
                throw new FormatException("Invalid stamp");
            }

            int version = reader.ReadByte();
            int type = reader.ReadByte();

            if (version != Version || type != Type) {
                throw new FormatException($"Invalid version/type: {version}.{type}");
            }

            ushort numSprites = reader.ReadUInt16();
            ushort dsigOffset = reader.ReadUInt16();
            var sprites = new NodeContainerFormat();

            using var dsigBinary = new BinaryFormat(source.Stream, dsigOffset, source.Stream.Length - dsigOffset);
            Dig image = digConverter.Convert(dsigBinary);

            var segmentsInfo = new DataStream();
            var yamlWriter = new TextDataWriter(segmentsInfo);
            spriteCollection = new List<Sprite>();

            for (int i = 0; i < numSprites; i++) {
                Sprite sprite = ReadSprite(reader);

                switch (image.Swizzling) {
                    case DigSwizzling.Tiled:
                        for (int j = 0; j < sprite.Segments.Count; j++) {
                            sprite.Segments[j].Layer = sprite.Segments.Count - j;
                        }

                        sprites.Root.Add(new Node($"sp_{i:00}", sprite));
                        break;
                    case DigSwizzling.Linear:
                        sprites.Root.Add(new Node($"tx_{i:00}", CreateTexture(sprite, image)));
                        spriteCollection.Add(sprite);
                        break;
                    default:
                        throw new FormatException("Invalid swizzling");
                }
            }

            // Export Segment Info to a YAML file so we can make edits later
            ISerializer serializer = new SerializerBuilder()
                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                    .Build();
            string yaml = serializer.Serialize(spriteCollection);
            yamlWriter.WriteLine(yaml);

            var container = new NodeContainerFormat();
            container.Root.Add(new Node("sprites", sprites));
            container.Root.Add(new Node("image", image));
            container.Root.Add(new Node("yaml", new BinaryFormat(segmentsInfo)));

            return container;
        }

        /// <summary>
        /// Deserializes a YAML string into a list of <see cref="Sprite"/>.
        /// </summary>
        /// <param name="yaml">The YAML string to deserialize.</param>
        /// <returns>A list of <see cref="Sprite"/>.</returns>
        public static List<Sprite> DeserializeYaml(string yaml)
        {
            return new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .WithTypeMapping<IImageSegment, ImageSegment>()
                .Build()
                .Deserialize<List<Sprite>>(yaml);
        }

        private static Dig CreateTexture(Sprite sprite, Dig fullImage)
        {
            var frame = new Dig(fullImage) {
                Pixels = new IndexedPixel[256 * 256],
                Width = 256,
                Height = 256,
            };

            foreach (IImageSegment seg in sprite.Segments) {
                var subImage = new Dig(fullImage, seg.Width, seg.Height, seg.TileIndex);
                frame.PasteImage(subImage, seg.CoordinateX, seg.CoordinateY, seg.HorizontalFlip, seg.VerticalFlip, seg.PaletteIndex);
            }

            return frame;
        }

        private static (int Width, int Height) GetSize(int shape)
        {
            int[] sizes = new int[4] { 8, 16, 32, 64 };

            return shape switch {
                0x00 => (sizes[0], sizes[0]),   // 1x1
                0x01 => (sizes[1], sizes[1]),   // 2x2
                0x02 => (sizes[2], sizes[2]),   // 4x4
                0x03 => (sizes[3], sizes[3]),   // 8x8
                0x04 => (sizes[1], sizes[0]),   // 2x1
                0x05 => (sizes[2], sizes[0]),   // 4x1
                0x06 => (sizes[2], sizes[1]),   // 4x2
                0x07 => (sizes[3], sizes[2]),   // 8x4
                0x08 => (sizes[0], sizes[1]),   // 1x2
                0x09 => (sizes[0], sizes[2]),   // 1x4
                0x0A => (sizes[1], sizes[2]),   // 2x4
                0x0B => (sizes[2], sizes[3]),   // 4x8
                _ => throw new FormatException($"Unknown size: 0x{shape:X2}"),
            };
        }

        private static (bool HFlip, bool VFlip) GetFlip(int shape)
        {
            return shape switch {
                0x00 => (false, false),
                0x01 => (true, false),
                0x02 => (false, true),
                0x03 => (true, true),
                _ => throw new FormatException($"Unknown flip: 0x{shape:X2}"),
            };
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
                (int width, int height) = GetSize(shape & 0x0F);
                (bool hFlip, bool vFlip) = GetFlip(shape >> 4);

                var segment = new ImageSegment() {
                    TileIndex = tileIndex,
                    CoordinateX = xPos,
                    CoordinateY = yPos,
                    PaletteIndex = paletteIndex,
                    Width = width,
                    Height = height,
                    VerticalFlip = vFlip,
                    HorizontalFlip = hFlip,
                };
                sprite.Segments.Add(segment);
            }

            sprite.Width = 256;
            sprite.Height = 256;
            reader.Stream.PopPosition();

            return sprite;
        }
    }
}
