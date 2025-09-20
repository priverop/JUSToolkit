using System;
using System.Collections.Generic;
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using Texim.Formats;
using Texim.Images;
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
        private List<SpriteDummy> spriteCollection;

        private string SegmentsOutputPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryToDtx3"/> class.
        /// </summary>
        /// <param name="segmentsOutputPath">Path to export segments.</param>
        public BinaryToDtx3(string segmentsOutputPath)
        {
            SegmentsOutputPath = segmentsOutputPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryToDtx3"/> class.
        /// </summary>
        public BinaryToDtx3()
        {
            SegmentsOutputPath = null;
        }

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
            spriteCollection = new List<SpriteDummy>();

            for (int i = 0; i < numSprites; i++) {
                switch (image.Swizzling) {
                    case DigSwizzling.Tiled:
                        sprites.Root.Add(new Node($"sp_{i:00}", ReadSprite(reader, image, i)));
                        break;
                    case DigSwizzling.Linear:
                        sprites.Root.Add(new Node($"tx_{i:00}", ReadTexture(reader, image, i)));
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

        private Sprite ReadSprite(DataReader reader, Dig fullImage, int index)
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
                    Layer = numSegments - i,
                };
                sprite.Segments.Add(segment);

                if (SegmentsOutputPath != null) {
                    var segment2Indexed = new ImageSegment2IndexedImage(new ImageSegment2IndexedImageParams {
                        FullImage = fullImage,
                        IsTiled = true,
                    });

                    FullImage segmentImage = segment2Indexed.Convert(segment).CreateFullImage(fullImage, false);

                    BinaryFormat pngSegment = new FullImage2Bitmap().Convert(segmentImage);
                    pngSegment.Stream.WriteTo($"{SegmentsOutputPath}/segments/{index}_segment{i}.png");
                }
            }

            sprite.Width = 256;
            sprite.Height = 256;
            reader.Stream.PopPosition();

            return sprite;
        }

        // ToDo: I guess we could merge these two methods and add some ifs?

        // These false sprites (we call them Textures) have a Linear image, so we can't
        // use the Texim Sprite system. That's why we compose regular images.
        private Dig ReadTexture(DataReader reader, Dig fullImage, int index)
        {
            var frame = new Dig(fullImage) {
                Pixels = new IndexedPixel[256 * 256],
                Width = 256,
                Height = 256,
            };

            int spriteOffset = reader.ReadUInt16() + PointerOffset;
            reader.Stream.PushToPosition(spriteOffset);
            ushort numSegments = reader.ReadUInt16();
            var sprite = new SpriteDummy();

            for (int i = 0; i < numSegments; i++) {
                ushort tileIndex = reader.ReadUInt16();
                sbyte xPos = reader.ReadSByte();
                sbyte yPos = reader.ReadSByte();
                byte shape = reader.ReadByte();
                byte paletteIndex = reader.ReadByte();
                (int width, int height) = GetSize(shape & 0x0F);
                (bool hFlip, bool vFlip) = GetFlip(shape >> 4);

                var segment = new Dig(fullImage, width, height, tileIndex);

                if (SegmentsOutputPath != null) {
                    var indexedImageParams = new IndexedImageBitmapParams {
                        Palettes = segment,
                    };

                    BinaryFormat a = new IndexedImage2Bitmap(indexedImageParams).Convert(segment);
                    a.Stream.WriteTo($"{SegmentsOutputPath}/segments/{index}_segment{i}.png");
                }

                frame.PasteImage(segment, xPos, yPos, hFlip, vFlip, paletteIndex);

                // Export Segment Info to a YAML file so we can make edits later
                var imageSegment = new ImageSegment() {
                    TileIndex = tileIndex,
                    CoordinateX = xPos,
                    CoordinateY = yPos,
                    PaletteIndex = paletteIndex,
                    Width = width,
                    Height = height,
                    VerticalFlip = vFlip,
                    HorizontalFlip = hFlip,
                };

                sprite.Segments.Add(imageSegment);
            }

            reader.Stream.PopPosition();
            spriteCollection.Add(sprite);

            return frame;
        }

        private (int width, int height) GetSize(int shape)
        {
            int[] sizes = new int[4] { 8, 16, 32, 64 };

            return shape switch {
                0x00 => (sizes[0], sizes[0]),   // 1x1 tiles = 8x8 pixels
                0x01 => (sizes[1], sizes[1]),   // 2x2 tiles = 16x16 pixels
                0x02 => (sizes[2], sizes[2]),   // 4x4 tiles = 32x32 pixels
                0x03 => (sizes[3], sizes[3]),   // 8x8 tiles = 64x64 pixels
                0x04 => (sizes[1], sizes[0]),   // 2x1 tiles = 16x8 pixels
                0x05 => (sizes[2], sizes[0]),   // 4x1 tiles = 32x8 pixels
                0x06 => (sizes[2], sizes[1]),   // 4x2 tiles = 32x16 pixels
                0x07 => (sizes[3], sizes[2]),   // 8x4 tiles = 64x32 pixels
                0x08 => (sizes[0], sizes[1]),   // 1x2 tiles = 8x16 pixels
                0x09 => (sizes[0], sizes[2]),   // 1x4 tiles = 8x32 pixels
                0x0A => (sizes[1], sizes[2]),   // 2x4 tiles = 16x32 pixels
                0x0B => (sizes[2], sizes[3]),   // 4x8 tiles = 32x64 pixels
                _ => throw new FormatException($"Unknown size: 0x{shape:X2}")
            };
        }

        private (bool hFlip, bool vFlip) GetFlip(int shape)
        {
            return shape switch {
                0x00 => (false, false),
                0x01 => (true, false),
                0x02 => (false, true),
                0x03 => (true, true),
                _ => throw new FormatException($"Unknown flip: 0x{shape:X2}")
            };
        }
    }
}
