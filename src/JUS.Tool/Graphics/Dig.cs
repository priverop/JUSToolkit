using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Texim.Compressions.Nitro;
using Texim.Images;
using Texim.Palettes;
using Texim.Pixels;
using Yarhl.FileFormat;

namespace JUSToolkit.Graphics
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
    /// Swizzling of a <see cref="Dig"/> image.
    /// </summary>
    public enum DigSwizzling
    {
        /// <summary>
        /// Tiled swizzling
        /// </summary>
        Tiled = 1,

        /// <summary>
        /// Linear swizzling
        /// </summary>
        Linear = 2,
    }

    /// <summary>
    /// Image format.
    /// </summary>
    public class Dig : IndexedPaletteImage
    {
        /// <summary>
        /// The Magic ID of the file.
        /// </summary>
        public const string STAMP = "DSIG";

        /// <summary>
        /// Initializes a new instance of the <see cref="Dig"/> class.
        /// </summary>
        public Dig()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dig"/> class cloning another Dig object.
        /// </summary>
        /// <param name="dig">Dig object to clone.</param>
        public Dig(Dig dig)
        {
            Unknown = dig.Unknown;
            ImageFormat = dig.ImageFormat;
            NumPaletteLines = dig.NumPaletteLines;
            Width = dig.Width;
            Height = dig.Height;
            Pixels = dig.Pixels;
            PaletteStart = dig.PaletteStart;
            PixelsStart = dig.PixelsStart;
            Bpp = dig.Bpp;
            Swizzling = dig.Swizzling;
            foreach (IPalette p in dig.Palettes) {
                Palettes.Add(p);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dig"/> class cloning the indexed image.
        /// </summary>
        /// <param name="dig">Dig object to clone.</param>
        /// <param name="image">IndexedImage object to clone.</param>
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
        public Dig(Dig dig, int width, int height, int tileIndex)
            : this(dig)
        {
            IIndexedPixelEncoding encoding;
            int size, totalWidth, nWidth, xTileIndex, yTileIndex;
            Height = height;
            Width = width;
            switch (dig.Bpp) {
                case DigBpp.Bpp4:
                    encoding = Indexed4Bpp.Instance;
                    size = width * height / 2;
                    nWidth = width / 2;
                    totalWidth = dig.Width / 2;
                    yTileIndex = tileIndex / (totalWidth / 4) * 8;
                    xTileIndex = (tileIndex % (totalWidth / 4)) * 4;
                    break;
                case DigBpp.Bpp8:
                    encoding = Indexed4Bpp.Instance;
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
        /// Gets or sets the first byte of the format. Maybe the Type?.
        /// </summary>
        public byte Unknown { get; set; }

        /// <summary>
        /// Gets or sets the ImageFormat.
        /// </summary>
        public byte ImageFormat { get; set; }

        /// <summary>
        /// Gets or sets the NumPaletteLines.
        /// </summary>
        public ushort NumPaletteLines { get; set; }

        /// <summary>
        /// Gets or sets the PaletteStart value.
        /// </summary>
        public uint PaletteStart { get; set; }

        /// <summary>
        /// Gets or sets the PixelsStart value.
        /// </summary>
        public uint PixelsStart { get; set; }

        /// <summary>
        /// Gets or sets the Bpp mode.
        /// </summary>
        public DigBpp Bpp { get; set; }

        /// <summary>
        /// Gets or sets the Swizzling mode.
        /// </summary>
        public DigSwizzling Swizzling { get; set; }

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
                    if (pixel.Alpha == 0 || pixel.Index == 0) {
                        continue;
                    }

                    int outIdx = ((yPos + 128 + y) * Width) + xPos + 128 + x;
                    Pixels[outIdx] = pixel;
                }
            }
        }

        /// <summary>
        /// Insert a transparent tile to the beginning of the dig and modify its map accordingly.
        /// </summary>
        /// <param name="map">Map to modify.</param>
        /// <returns>The <see cref="Dig"/> with the transparent tile.</returns>
        public Dig InsertTransparentTile(ScreenMap map)
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
                Pixels[i] = new IndexedPixel(Pixels[i].Index, Pixels[i].Alpha, paletteIndex);
        }
    }
}
