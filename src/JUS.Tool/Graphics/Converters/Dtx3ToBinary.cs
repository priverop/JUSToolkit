using System;
using System.Collections.Generic;
using JUSToolkit.Graphics.Converters;
using Texim.Sprites;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Graphics.Converters
{
    /// <summary>
    /// Converter from DTX3 format to binary format.
    /// </summary>
    public class Dtx3ToBinary : IConverter<NodeContainerFormat, BinaryFormat>
    {
        private const string Stamp = "DSTX";
        private const byte Version = 0x01;
        private const byte Type = 0x03;

        /// <summary>
        /// Converts a DTX3 format to a binary format.
        /// </summary>
        /// <param name="dtx">The DTX3 format to convert.</param>
        /// <returns>The converted binary format.</returns>
        public BinaryFormat Convert(NodeContainerFormat dtx)
        {
            var bin = new BinaryFormat();
            var writer = new DataWriter(bin.Stream);

            writer.Write(Stamp, false);
            writer.Write(Version);
            writer.Write(Type);

            NavigableNodeCollection<Node> sprites = dtx.Root.Children["sprites"].Children;
            writer.Write((ushort)sprites.Count);
            writer.WriteOfType<ushort>(0x00);

            ushort offset = (ushort)(sprites.Count * 2);
            foreach (Node n in sprites) {
                writer.Write(offset);
                offset += (ushort)(2 + (n.GetFormatAs<Sprite>().Segments.Count * 6));
            }

            foreach (Node n in sprites) {
                Sprite sprite = n.GetFormatAs<Sprite>();
                writer.Write((ushort)sprite.Segments.Count);
                foreach (IImageSegment s in sprite.Segments) {
                    writer.Write((ushort)s.TileIndex);
                    writer.Write((sbyte)s.CoordinateX);
                    writer.Write((sbyte)s.CoordinateY);
                    writer.Write((byte)(GetSize(s.Width, s.Height) + GetFlip(s.HorizontalFlip, s.VerticalFlip)));
                    writer.Write(s.PaletteIndex);
                }
            }

            offset = (ushort)writer.Stream.Position;
            writer.Stream.PushToPosition(0x08);
            writer.Write(offset);
            writer.Stream.PopPosition();

            var reader = new DataReader(dtx.Root.Children["image"].TransformWith<Dig2Binary>()
                .GetFormatAs<BinaryFormat>().Stream);
            reader.Stream.Position = 0;
            writer.Write(reader.ReadBytes((int)reader.Stream.Length));

            return bin;
        }

        /// <summary>
        /// Gets the size byte based on width and height.
        /// </summary>
        /// <param name="width">The width of the segment.</param>
        /// <param name="height">The height of the segment.</param>
        /// <returns>The size byte.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the size is invalid.</exception>
        private static byte GetSize(int width, int height)
        {
            return (width, height) switch {
                (8, 8) => 0x00,
                (16, 16) => 0x01,
                (32, 32) => 0x02,
                (64, 64) => 0x03,
                (16, 8) => 0x04,
                (32, 8) => 0x05,
                (32, 16) => 0x06,
                (64, 32) => 0x07,
                (8, 16) => 0x08,
                (8, 32) => 0x09,
                (16, 32) => 0x0A,
                (32, 64) => 0x0B,
                _ => throw new ArgumentOutOfRangeException(nameof(width), $"Invalid size: {width}x{height}")
            };
        }

        /// <summary>
        /// Gets the flip byte based on horizontal and vertical flip.
        /// </summary>
        /// <param name="hFlip">Indicates if horizontal flip is applied.</param>
        /// <param name="vFlip">Indicates if vertical flip is applied.</param>
        /// <returns>The flip byte.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the flip combination is invalid.</exception>
        private static byte GetFlip(bool hFlip, bool vFlip)
        {
            return (hFlip, vFlip) switch {
                (false, false) => 0x00,
                (true, false) => 0x10,
                (false, true) => 0x20,
                (true, true) => 0x30,
            };
        }
    }
}
