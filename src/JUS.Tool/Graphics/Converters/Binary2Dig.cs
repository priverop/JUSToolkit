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
            if (reader.ReadString(4) != Dig.Stamp) {
                throw new FormatException("Invalid stamp");
            }

            byte version = reader.ReadByte();
            byte flags = reader.ReadByte();
            var bpp = (DigBpp)(flags & 0x0F);
            var dataFormat = (DigDataFormat)(flags >> 4);
            byte paletteCount = reader.ReadByte();
            var colorFormat = (DigColorFormat)reader.ReadByte();
            ushort field08 = reader.ReadUInt16();
            ushort field0A = reader.ReadUInt16();

            // Palettes have always 16 colors per palette, but in 8bpp they are combined into a single big palette
            int colorsPerPalette = bpp == DigBpp.Bpp8 ? paletteCount * 16 : 16;
            int actualPaletteCount = bpp == DigBpp.Bpp8 ? 1 : paletteCount;
            DigColorFormat actualColorFormat = forceSupportAlpha ? DigColorFormat.Abgr555 : colorFormat;
            IColorEncoding colorEncoding = actualColorFormat switch {
                DigColorFormat.Bgr555 => Bgr555Encoding.Instance,
                DigColorFormat.Abgr555 => Abgr555Encoding.Instance,
                _ => throw new FormatException($"Unknown color format: {colorFormat}"),
            };

            // The format 4 (compressed block) seems to use the alpha bit
            var palettes = new Collection<IPalette>();
            for (int i = 0; i < actualPaletteCount; i++) {
                Rgb[] colors = colorEncoding.DecodeExactly(reader.Stream, colorsPerPalette);
                palettes.Add(new Palette(colors));
            }

            uint unkBlockValue = 0;
            if (version != 1 && dataFormat is DigDataFormat.CompressedBlocks) {
                unkBlockValue = reader.ReadUInt32();
            }

            Stream pixelData = reader.Stream.Slice(reader.Stream.Position);
            if (dataFormat is DigDataFormat.CompressedImage) {
                pixelData = new LzssDecoder().Convert(pixelData);
            }

            long pixelCount = bpp == DigBpp.Bpp4 ? pixelData.Length * 2 : pixelData.Length;

            // In the case of compressed blocks, there isn't the concept of width.
            // We generate an always valid one.
            int width, height;
            if (dataFormat == DigDataFormat.CompressedBlocks) {
                // FUTURE: recalculate by pixel count after decompressing the blocks.
                width = 8;
                height = (int)(pixelCount / width);
            } else {
                width = field08;
                height = field0A;
            }

            // DSIG inside DTX may have width and height values that doesn't represent the amount of pixels.
            // These images do not really have a size, as they have segments to reconstruct a different image.
            // We calculate a valid size for them based on a minimal width (tile width).
            var originalSize = new Size(width, height);
            if (dataFormat != DigDataFormat.CompressedBlocks && (width * height) != pixelCount) {
                width = 8;
                height = (int)(pixelCount / width);
            }

            byte[][] compressedSegments = [];
            IndexedPixel[] pixels = [];
            if (dataFormat is DigDataFormat.CompressedBlocks) {
                compressedSegments = ReadCompressedBlocks(reader);
            } else {
                pixelData.Position = 0;
                pixels = ReadPixels(pixelData, width, height, bpp, dataFormat);
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
                CompressedSegments = compressedSegments,
            };
            return dig;
        }

        private static IndexedPixel[] ReadPixels(Stream stream, int width, int height, DigBpp bpp, DigDataFormat format)
        {
            if (width == 0 || height == 0) {
                return [];
            }

            IIndexedPixelEncoding pixelEncoding = bpp switch {
                DigBpp.Bpp4 => Indexed4BppEncoding.Instance,
                DigBpp.Bpp8 => Indexed8BppEncoding.Instance,
                _ => throw new FormatException($"Unsupported BPP {bpp}"),
            };

            IndexedPixel[] pixels = pixelEncoding.DecodeExactly(stream, width * height);

            if (format is DigDataFormat.Tiled) {
                return new TileSwizzling<IndexedPixel>(width).Unswizzle(pixels);
            }

            return pixels;
        }

        private static byte[][] ReadCompressedBlocks(DataReader reader)
        {
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
