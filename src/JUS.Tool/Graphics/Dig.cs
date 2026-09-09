using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using JUS.Tool.Utils;
using Texim.Images;
using Texim.Palettes;
using Texim.Pixels;
using Texim.TileMaps;

namespace JUS.Tool.Graphics
{
    /// <summary>
    /// Bpp of a <see cref="Dig"/> image.
    /// </summary>
    public enum DigBpp
    {
        /// <summary>
        /// 4 bpp mode.
        /// </summary>
        Bpp4 = 0,

        /// <summary>
        /// 8 bpp mode.
        /// </summary>
        Bpp8 = 1,
    }

    /// <summary>
    /// Format of the data of an <see cref="Dig"/> image.
    /// </summary>
    public enum DigDataFormat
    {
        /// <summary>
        /// Invalid image format.
        /// </summary>
        None = 0,

        /// <summary>
        /// Tiled swizzling.
        /// </summary>
        Tiled = 1,

        /// <summary>
        /// Linear swizzling.
        /// </summary>
        Linear = 2,

        /// <summary>
        /// Unknown format, not used on this game.
        /// </summary>
        Unknown3 = 3,

        /// <summary>
        /// Blocks of compressed data.
        /// </summary>
        CompressedBlocks = 4,

        /// <summary>
        /// One block of compressed data.
        /// </summary>
        CompressedImage = 5,
    }

    /// <summary>
    /// Format of the color encoding in DSIG palettes.
    /// </summary>
    public enum DigColorFormat
    {
        /// <summary>
        /// BGR555 with no transparency.
        /// </summary>
        Bgr555 = 0,

        /// <summary>
        /// ABGR555 with one bit transparency.
        /// </summary>
        Abgr555 = 4,
    }

    /// <summary>
    /// Information about a compressed DSIG type 4 block.
    /// </summary>
    /// <param name="Index">The block index.</param>
    /// <param name="PixelStart">The index of the first pixel in the block.</param>
    /// <param name="PixelCount">The number of pixels in the block.</param>
    public sealed record DigBlockInfo(int Index, int PixelStart, int PixelCount);

    /// <summary>
    /// Image format.
    /// </summary>
    public class Dig : IndexedPaletteImage
    {
        /// <summary>
        /// The Magic ID of the file.
        /// </summary>
        public const string Stamp = "DSIG";

        /// <summary>
        /// 10 bits for the tile index.
        /// </summary>
        public const int MaxTiles = 1024;

        /// <summary>
        /// Pixels in a tile (8x8).
        /// </summary>
        public const int PixelsPerTile = 64;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dig"/> class.
        /// </summary>
        public Dig()
        {
            BlocksInfo = [];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dig"/> class cloning another Dig object.
        /// </summary>
        /// <param name="dig">Dig object to clone.</param>
        [SetsRequiredMembers]
        public Dig(Dig dig)
        {
            Version = dig.Version;
            Bpp = dig.Bpp;
            DataFormat = dig.DataFormat;
            Width = dig.Width;
            Height = dig.Height;
            OriginalSize = dig.OriginalSize;
            FormatColorEncoding = dig.FormatColorEncoding;
            ActualColorEncodingFormat = dig.ActualColorEncodingFormat;
            UnknownBlockValue = dig.UnknownBlockValue;
            BlocksInfo = dig.BlocksInfo.ToArray();
            Pixels = dig.Pixels.ToArray();
            Palettes = new Collection<IPalette>(dig.Palettes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dig"/> class cloning the indexed image.
        /// </summary>
        /// <param name="dig">Dig object to clone.</param>
        /// <param name="image">Indexed image object to clone.</param>
        [SetsRequiredMembers]
        public Dig(Dig dig, IIndexedImage image)
            : this(dig)
        {
            Height = image.Height;
            Width = image.Width;
            Pixels = image.Pixels.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dig"/> class creating a subimage.
        /// </summary>
        /// <param name="dig"><see cref="Dig"/> image to create subimage from.</param>
        /// <param name="width">Width of the subimage.</param>
        /// <param name="height">Height of the subimage.</param>
        /// <param name="tileIndex">Tile index where the subimage starts from.</param>
        /// <exception cref="FormatException"><paramref name="dig"/> doesn't have a valid format.</exception>
        [SetsRequiredMembers]
        public Dig(Dig dig, int width, int height, int tileIndex)
            : this(dig)
        {
            IIndexedPixelEncoding encoding;
            int size, totalWidth, nWidth, xTileIndex, yTileIndex;
            Height = height;
            Width = width;
            switch (dig.Bpp) {
                case DigBpp.Bpp4:
                    encoding = Indexed4BppEncoding.Instance;
                    size = width * height / 2;
                    nWidth = width / 2;
                    totalWidth = dig.Width / 2;
                    yTileIndex = tileIndex / (totalWidth / 4) * 8;
                    xTileIndex = (tileIndex % (totalWidth / 4)) * 4;
                    break;
                case DigBpp.Bpp8:
                    encoding = Indexed8BppEncoding.Instance;
                    size = width * height;
                    nWidth = width;
                    totalWidth = dig.Width;
                    xTileIndex = (tileIndex % (totalWidth / 8)) * 8;
                    yTileIndex = dig.Height / (totalWidth / 8) * 8;
                    break;
                default:
                    throw new FormatException($"Invalid bpp: {dig.Bpp}");
            }

            byte[] rawPixels = new byte[size];
            byte[] encoded = encoding.Encode(dig.Pixels);

            int idx = 0;
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < nWidth; x++) {
                    int fullIndex = ((y + yTileIndex) * totalWidth) + x + xTileIndex;
                    rawPixels[idx++] = encoded[fullIndex];
                }
            }

            Pixels = encoding.Decode(rawPixels);
        }

        /// <summary>
        /// Gets or sets the format version.
        /// </summary>
        public byte Version { get; set; }

        /// <summary>
        /// Gets or sets the bits per pixel (pixel encoding).
        /// </summary>
        public DigBpp Bpp { get; set; }

        /// <summary>
        /// Gets or sets the format of the pixel data.
        /// </summary>
        public DigDataFormat DataFormat { get; set; }

        /// <summary>
        /// Gets or sets the palette color encoding as specified in the DSIG binary format.
        /// </summary>
        public DigColorFormat FormatColorEncoding { get; set; }

        /// <summary>
        /// Gets or sets the actual palette encoding format that DSTX may overwrite.
        /// </summary>
        public DigColorFormat ActualColorEncodingFormat { get; set; }

        /// <summary>
        /// Gets or sets the unknown value from compressed blocks in version 2.
        /// </summary>
        public uint UnknownBlockValue { get; set; }

        /// <summary>
        /// Gets or sets the information about the blocks, if any.
        /// </summary>
        public DigBlockInfo[] BlocksInfo { get; set; }

        /// <summary>
        /// Gets or sets the original size in the binary format that doesn't match the pixel count.
        /// </summary>
        /// <remarks>Probably the size before DSTX compression.</remarks>
        public Size OriginalSize { get; set; }

        /// <summary>
        /// Paste a <see cref="Dig"/> subimage into this <see cref="Dig"/>.
        /// </summary>
        /// <param name="subimage"><see cref="Dig"/> subimage.</param>
        /// <param name="xPos">Starting X position where the subimage will be pasted.</param>
        /// <param name="yPos">Starting Y position where the subimage will be pasted.</param>
        /// <param name="horizontalFlip">Flip the subimage horizontally.</param>
        /// <param name="verticalFlip">Flip the subimage vertically.</param>
        /// <param name="paletteIndex">Palette index of the subimage.</param>
        public void PasteImage(Dig subimage, int xPos, int yPos, bool horizontalFlip, bool verticalFlip, byte paletteIndex)
        {
            if (horizontalFlip)
                subimage.FlipHorizontal();
            if (verticalFlip)
                subimage.FlipVertical();

            subimage.SetPalette(paletteIndex);

            for (int x = 0; x < subimage.Width; x++) {
                for (int y = 0; y < subimage.Height; y++) {
                    int inIdx = (y * subimage.Width) + x;
                    IndexedPixel pixel = subimage.Pixels[inIdx];
                    if (pixel.Alpha == 0 || pixel.ColorIndex == 0) {
                        continue;
                    }

                    int outIdx = ((yPos + 128 + y) * Width) + xPos + 128 + x;
                    Pixels[outIdx] = pixel;
                }
            }
        }

        /// <summary>
        /// Checks if the image has more than MaxTiles.
        /// </summary>
        /// <param name="name">Name of the file.</param>
        public void CheckMaxTiles(string name)
        {
            int tiles = Pixels.Length / PixelsPerTile;
            if (tiles > MaxTiles) {
                Logger.DisplayError($"MaxTiles ({MaxTiles}) reached in {name}");
            }
        }

        /// <summary>
        /// Insert a transparent tile to the beginning of the dig and modify its map accordingly.
        /// </summary>
        /// <param name="map">Map to modify.</param>
        /// <returns>The <see cref="Dig"/> with the transparent tile.</returns>
        public Dig InsertTransparentTile(ITileMap map)
        {
            var dig = new Dig(this) {
                Pixels = new IndexedPixel[this.Pixels.Length + 64],
                Height = this.Height + 8,
            };

            dig.PasteImage(this, -128, -120, false, false, 0);
            for (int i = 0; i < map.Maps.Length; i++) {
                map.Maps[i] = new MapInfo() {
                    HorizontalFlip = map.Maps[i].HorizontalFlip,
                    VerticalFlip = map.Maps[i].VerticalFlip,
                    TileIndex = (short)(map.Maps[i].TileIndex + 1),
                    PaletteIndex = map.Maps[i].PaletteIndex,
                };
            }

            return dig;
        }

        /// <summary>
        /// Insert a transparent tile to the beginning of the dig.
        /// </summary>
        /// <returns>The <see cref="Dig"/> with the transparent tile.</returns>
        public Dig InsertTransparentTile()
        {
            var newPixels = new IndexedPixel[64]; // 8x8

            return new Dig(this) {
                Pixels = newPixels.Concat(Pixels).ToArray(),
                Height = this.Height + 8,
            };
        }

        /// <summary>
        /// Flip pixels horizontally.
        /// </summary>
        public void FlipHorizontal()
        {
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width / 2; x++) {
                    int t1 = (y * Width) + x;
                    int t2 = (y * Width) + (Width - 1 - x);

                    IndexedPixel swap = Pixels[t1];
                    Pixels[t1] = Pixels[t2];
                    Pixels[t2] = swap;
                }
            }
        }

        /// <summary>
        /// Flip pixels vertically.
        /// </summary>
        public void FlipVertical()
        {
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height / 2; y++) {
                    int t1 = x + (Width * y);
                    int t2 = x + (Width * (Height - 1 - y));

                    IndexedPixel swap = Pixels[t1];
                    Pixels[t1] = Pixels[t2];
                    Pixels[t2] = swap;
                }
            }
        }

        /// <summary>
        /// Sets palette index for all pixels.
        /// </summary>
        /// <param name="paletteIndex">Palette index.</param>
        public void SetPalette(byte paletteIndex)
        {
            for (int i = 0; i < Pixels.Length; i++)
                Pixels[i] = new IndexedPixel(Pixels[i].ColorIndex, Pixels[i].Alpha, paletteIndex);
        }
    }

    internal static class DigExtensions
    {
        extension(DigBpp bpp)
        {
            public IIndexedPixelEncoding GetPixelEncoding() => bpp switch {
                DigBpp.Bpp4 => Indexed4BppEncoding.Instance,
                DigBpp.Bpp8 => Indexed8BppEncoding.Instance,
                _ => throw new FormatException($"Invalid bpp: {bpp}"),
            };
        }
    }
}
