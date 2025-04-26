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
    /// Converter from DTX4 format to binary format.
    /// </summary>
    public class Dtx4ToBinary : IConverter<NodeContainerFormat, BinaryFormat>
    {
        private const string Stamp = "DSTX";
        private const byte Version = 0x01;
        private const byte Type = 0x04;

        /// <summary>
        /// Converts a DTX4 format to a binary format.
        /// </summary>
        /// <param name="dtx">The DTX4 format to convert.</param>
        /// <returns>The converted binary format.</returns>
        public BinaryFormat Convert(NodeContainerFormat dtx)
        {
            var bin = new BinaryFormat();
            var writer = new DataWriter(bin.Stream);

            writer.Write(Stamp, false);
            writer.Write(Version);
            writer.Write(Type);

            Sprite sprite = dtx.Root.Children["sprite"].GetFormatAs<Sprite>();

            // NumSegments
            writer.Write((ushort)sprite.Segments.Count);

            // dsigOffset: 2bytes dsigOffset + 2bytes uknown + 4*numSegments
            long dsigOffset = writer.Stream.Position + 4 + (sprite.Segments.Count * 4);
            writer.Write((ushort)dsigOffset);

            // unknown
            writer.WriteOfType<ushort>(0x01);

            // WriteSprite
            foreach (IImageSegment s in sprite.Segments) {
                writer.Write((sbyte)(s.Width / 8));
                writer.Write((sbyte)(s.Height / 8));
                writer.Write((ushort)s.TileIndex);
            }

            // WriteDig
            var reader = new DataReader(dtx.Root.Children["image"].TransformWith<Dig2Binary>()
                .GetFormatAs<BinaryFormat>().Stream);
            reader.Stream.Position = 0;
            writer.Write(reader.ReadBytes((int)reader.Stream.Length));

            return bin;
        }
    }
}
