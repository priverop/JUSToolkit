// Copyright (c) 2022 Priverop

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

using System.Collections.ObjectModel;
using System.Drawing;
using JUS.Tool.Framework;
using SceneGate.Ekona.Compression;
using Texim.Colors;
using Texim.Palettes;
using Texim.Pixels;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUS.Tool.Graphics.Converters
{
    /// <summary>
    /// Converts between BinaryFormat (a file) containing a Dsig Format and IndexedPaletteImage (PNG).
    /// </summary>
    public class Binary2Dig : IConverter<IBinary, Dig>
    {
        private readonly bool forceSupportAlpha;

        /// <summary>
        /// Creates a new instance of the <see cref="Binary2Dig"/> class without
        /// overwriting the support of alpha channels.
        /// </summary>
        public Binary2Dig()
        {
            forceSupportAlpha = false;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Binary2Dig"/> class.
        /// </summary>
        /// <param name="forceSupportAlpha">
        /// Value that specifies whether the palette color encoding follows the DSIG header value or uses ABGR1555.
        /// </param>
        public Binary2Dig(bool forceSupportAlpha)
        {
            this.forceSupportAlpha = forceSupportAlpha;
        }

        /// <summary>
        /// Converts a <see cref="BinaryFormat"/> (file) to a <see cref="Dig"/>.
        /// </summary>
        /// <param name="source">File to convert.</param>
        /// <returns><see cref="Dig"/>.</returns>
        public Dig Convert(IBinary source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var reader = new DataReader(source.Stream);
            source.Stream.Position = 0;

            // Header
            FormatException.ThrowIfNotEqual(reader.ReadString(4), Dig.Stamp, "stamp");
            byte version = reader.ReadByte();
            byte flags = reader.ReadByte();
            var bpp = (DigBpp)(flags & 0x0F);
            var dataFormat = (DigDataFormat)(flags >> 4);
            byte paletteCount = reader.ReadByte();
            var colorFormat = (DigColorFormat)reader.ReadByte();
            ushort field08 = reader.ReadUInt16();
            ushort field0A = reader.ReadUInt16();

            // Palette
            // DSIG inside DSTX type 4 use ABGR555 no matter what the header says
            DigColorFormat actualColorFormat = forceSupportAlpha ? DigColorFormat.Abgr555 : colorFormat;
            Collection<IPalette> palettes = ReadPalettes(reader, paletteCount, actualColorFormat, bpp);

            uint unkBlockValue = 0;
            if (version != 1 && dataFormat is DigDataFormat.CompressedBlocks) {
                unkBlockValue = reader.ReadUInt32();
            }

            (IndexedPixel[] pixels, DigBlockInfo[] blocks) = ReadImageData(source.Stream, bpp, dataFormat);

            // In the case of compressed blocks, there isn't the concept of width. We generate an always valid one.
            int width, height;
            if (dataFormat == DigDataFormat.CompressedBlocks) {
                width = 8;
                height = pixels.Length / width;
            } else {
                width = field08;
                height = field0A;
            }

            // DSIG inside DTX may have width and height values that doesn't represent the amount of pixels.
            // These images do not really have a size, as they have segments to reconstruct a different image.
            // We calculate a valid size for them based on a minimal width (tile width).
            var originalSize = new Size(width, height);
            if (dataFormat != DigDataFormat.CompressedBlocks && (width * height) != pixels.Length) {
                width = 8;
                height = pixels.Length / width;
            }

            if (dataFormat is DigDataFormat.Tiled) {
                pixels = new TileSwizzling<IndexedPixel>(width).Unswizzle(pixels);
            }

            var dig = new Dig {
                Version = version,
                Bpp = bpp,
                DataFormat = dataFormat,
                Width = width,
                Height = height,
                OriginalSize = originalSize,
                FormatColorEncoding = colorFormat,
                ActualColorEncodingFormat = actualColorFormat,
                Pixels = pixels,
                Palettes = palettes,
                UnknownBlockValue = unkBlockValue,
                BlocksInfo = blocks,
            };
            return dig;
        }

        private static Collection<IPalette> ReadPalettes(DataReader reader, int paletteCount, DigColorFormat colorFormat, DigBpp bpp)
        {
            // Palettes have always 16 colors per palette, but in 8bpp they are combined into a single big palette
            int colorsPerPalette = bpp == DigBpp.Bpp8 ? paletteCount * 16 : 16;
            int actualPaletteCount = bpp == DigBpp.Bpp8 ? 1 : paletteCount;
            IColorEncoding colorEncoding = colorFormat switch {
                DigColorFormat.Bgr555 => Bgr555Encoding.Instance,
                DigColorFormat.Abgr555 => Abgr555Encoding.Instance,
                _ => throw new FormatException($"Unknown color format: {colorFormat}"),
            };

            var palettes = new Collection<IPalette>();
            for (int i = 0; i < actualPaletteCount; i++) {
                Rgb[] colors = colorEncoding.DecodeExactly(reader.Stream, colorsPerPalette);
                palettes.Add(new Palette(colors));
            }

            return palettes;
        }

        private static (IndexedPixel[], DigBlockInfo[]) ReadImageData(Stream stream, DigBpp bpp, DigDataFormat format)
        {
            if (format is DigDataFormat.CompressedImage) {
                // Decompress full block, and read as lineal.
                using Stream compressed = stream.Slice(stream.Position);
                using Stream decompressed = new LzssDecoder().Convert(compressed);
                decompressed.Position = 0;
                return ReadImageData(decompressed, bpp, DigDataFormat.Linear);
            }

            if (format is DigDataFormat.CompressedBlocks) {
                // Read each block, decompress them, and read as lineal.
                byte[][] compressedSegments = ReadCompressedBlocks(stream);

                List<IndexedPixel> mergedPixels = [];
                List<DigBlockInfo> blocks = [];
                for (int i = 0; i < compressedSegments.Length; i++) {
                    using var blockData = new MemoryStream(compressedSegments[i]);
                    (IndexedPixel[] blockPixels, _) = ReadImageData(blockData, bpp, DigDataFormat.CompressedImage);
                    blocks.Add(new DigBlockInfo(i, mergedPixels.Count, blockPixels.Length));
                    mergedPixels.AddRange(blockPixels);
                }

                return (mergedPixels.ToArray(), blocks.ToArray());
            }

            IndexedPixel[] pixels = ReadPixels(stream, bpp);
            return (pixels, []);
        }

        private static IndexedPixel[] ReadPixels(Stream stream, DigBpp bpp)
        {
            if (stream.EndOfStream) {
                return [];
            }

            byte[] data = stream.ReadBytes((int)(stream.Length - stream.Position));
            return bpp.GetPixelEncoding().Decode(data);
        }

        private static byte[][] ReadCompressedBlocks(Stream stream)
        {
            var reader = new DataReader(stream);
            long basePosition = reader.Stream.Position;
            int count = reader.ReadInt32();
            byte[][] compressedBlocks = new byte[count][];

            for (int i = 0; i < count; i++) {
                ushort encodedOffset = reader.ReadUInt16();
                ushort length = reader.ReadUInt16();

                long blockPosition = basePosition + (encodedOffset * 4);
                using (reader.Stream.EnterWithPosition(blockPosition)) {
                    compressedBlocks[i] = reader.ReadBytes(length);
                }
            }

            return compressedBlocks;
        }
    }
}
