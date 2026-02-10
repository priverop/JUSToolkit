using System;
using System.Linq;
using SixLabors.ImageSharp;
using Texim.Compressions.Nitro;
using Texim.Formats;
using Texim.Images;
using Texim.Palettes;
using Texim.Pixels;

namespace JUSToolkit.Graphics
{
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
        /// Initializes a new instance of the <see cref="Dig"/> class creating a subimage (segment).
        /// </summary>
        /// <param name="baseImage"><see cref="Dig"/> image to create subimage from.</param>
        /// <param name="segmentWidth">Width of the subimage.</param>
        /// <param name="segmentHeight">Height of the subimage.</param>
        /// <param name="startTileIndex">Tile index where the subimage starts from.</param>
        public Dig(Dig baseImage, int segmentWidth, int segmentHeight, int startTileIndex)
            : this(baseImage)
        {
            // (tile index / tilesPerRow) * tileWidth
            Size tileSize = new(8, 8);
            int tilesPerRow = baseImage.Width / tileSize.Width;
            int startX = (startTileIndex % tilesPerRow) * tileSize.Width;
            int startY = (startTileIndex / tilesPerRow) * tileSize.Height;

            IndexedImage subimage = baseImage.SubImage(startX, startY, segmentWidth, segmentHeight);

            Height = subimage.Height;
            Width = subimage.Width;
            Pixels = subimage.Pixels;
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
        /// Paste a <see cref="Dig"/> subimage into this <see cref="Dig"/>. CopySegment in Texim.
        /// </summary>
        /// <param name="subimage"><see cref="Dig"/> subimage.</param>
        /// <param name="xPos">Starting X position where the subimage will be pasted.</param>
        /// <param name="yPos">Starting Y position where the subimage will be pasted.</param>
        /// <param name="horizontalFlip">Flip the subimage horizontally.</param>
        /// <param name="verticalFlip">Flip the subimage vertically.</param>
        /// <param name="paletteIndex">Palette index of the subimage.</param>
        public void PasteImage(Dig subimage, int xPos, int yPos, bool horizontalFlip, bool verticalFlip, byte paletteIndex)
        {
            // Span<IndexedPixel> spanSegment = subimage.Pixels;

            if (horizontalFlip) {
                // spanSegment.FlipHorizontal(new Size(subimage.Width, subimage.Height));
                subimage.FlipHorizontal();
            }

            if (verticalFlip) {
                // spanSegment.FlipVertical(new Size(subimage.Width, subimage.Height));
                subimage.FlipVertical();
            }

            subimage.SetPalette(paletteIndex);

            Span<IndexedPixel> pixelsSpan = Pixels;

            CopySegment(subimage, pixelsSpan, Width, new Rectangle(128, 128, subimage.Width, subimage.Height), xPos, yPos);
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

        // TODO: Upgrade Texim to the last version to remove this method
        private void CopySegment(IIndexedImage segmentImage, Span<IndexedPixel> output, int width, Rectangle segmentInfo, int relativeX, int relativeY)
        {
            for (int x = 0; x < segmentInfo.Width; x++) {
                for (int y = 0; y < segmentInfo.Height; y++) {
                    int inIdx = (y * segmentInfo.Width) + x;
                    IndexedPixel pixel = segmentImage.Pixels[inIdx];
                    if (pixel.Alpha == 0 || pixel.Index == 0) {
                        continue;
                    }

                    int outIdx = ((relativeY + segmentInfo.Y + y) * width) + relativeX + segmentInfo.X + x;
                    output[outIdx] = pixel;
                }
            }
        }
    }
}
