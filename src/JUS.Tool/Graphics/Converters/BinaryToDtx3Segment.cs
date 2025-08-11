using System;
using System.Collections;
using System.Collections.Generic;
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using Texim.Formats;
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
    /// Exports a single Segment from a DTX file. Useful for testing.
    /// </summary>
    public class BinaryToDtx3Segment : IConverter<IBinary, NodeContainerFormat>
    {
        private const string Stamp = "DSTX";
        private const int Version = 0x01;
        private const int Type = 0x03;
        private const int PointerOffset = 0x0A;
        private readonly Binary2Dig digConverter = new();

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

            for (int i = 0; i < numSprites; i++) {
                Console.WriteLine();
                Console.WriteLine($"tx_{i:00}");
                sprites.Root.Add(new Node($"tx_{i:00}", ReadTexture(reader, image, i)));
                Console.WriteLine("-----");
            }

            var container = new NodeContainerFormat();
            container.Root.Add(new Node("segment", sprites));
            container.Root.Add(new Node("image", image));

            return container;
        }

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
            // Console.WriteLine($"numSegments: {numSegments}");
            SpriteDummy sprite = new SpriteDummy();

            for (int i = 0; i < numSegments; i++) {
                ushort tileIndex = reader.ReadUInt16();
                sbyte xPos = reader.ReadSByte();
                sbyte yPos = reader.ReadSByte();
                byte shape = reader.ReadByte();
                byte paletteIndex = reader.ReadByte();
                (int width, int height) = GetSize(shape & 0x0F);
                (bool hFlip, bool vFlip) = GetFlip(shape >> 4);
                // Console.WriteLine($"positions: {xPos} x {yPos}");
                // Console.WriteLine($"tileIndex: {tileIndex}");
                // Console.WriteLine($"shape: {shape}");
                // Console.WriteLine($"flip: {hFlip} x {vFlip}");
                // Console.WriteLine($"size: {width} x {height}");

                var segment = new Dig(fullImage, width, height, tileIndex);

                var indexedImageParams = new IndexedImageBitmapParams {
                    Palettes = segment,
                };

                BinaryFormat a = new IndexedImage2Bitmap(indexedImageParams).Convert(segment);
                a.Stream.WriteTo($"src/JUS.CLI/bin/Debug/net8.0/23-dtx3tx/segments_ko/{index}_segment{i}.png");
                // Console.WriteLine();
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

            return frame;
        }

        private (int width, int height) GetSize(int shape)
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
