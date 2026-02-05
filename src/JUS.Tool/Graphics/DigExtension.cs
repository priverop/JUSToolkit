using System;
using Texim.Pixels;

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

        /// <summary>
        /// 2 bpp mode.
        /// </summary>
        Bpp2 = 2,
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

    // To work with subimages
    public class DigExtension
    {
        public readonly struct DigSubimageParams
        {
            public int SubImageSizeInBytes { get; init; }

            public int SubImageWidthInBytes { get; init; }

            public int BaseImageWidthInBytes { get; init; }

            public int XTileIndex { get; init; }

            public int YTileIndex { get; init; }
        }

        public static IIndexedPixelEncoding GetEncoding(DigBpp bpp)
        {
            return bpp switch {
                DigBpp.Bpp2 => Indexed2Bpp.Instance,
                DigBpp.Bpp4 => Indexed4Bpp.Instance,
                DigBpp.Bpp8 => Indexed8Bpp.Instance,
                _ => throw new FormatException($"Invalid bpp: {bpp}")
            };
        }

        public static int GetPixelsPerByte(DigBpp bpp)
        {
            return bpp switch {
                DigBpp.Bpp2 => 4, // 2 bits per pixel = 4 pixels per byte
                DigBpp.Bpp4 => 2, // 4 bits per pixel = 2 pixels per byte
                DigBpp.Bpp8 => 1, // 8 bits per pixel = 1 pixel per byte
                _ => throw new FormatException($"Invalid bpp: {bpp}")
            };
        }

        public static DigSubimageParams GetSubImageParams(Dig baseImage, int subImageWidth, int subImageHeight, int startTileIndex)
        {
            const int TILESIZEINPIXELS = 8;

            int pixelsPerByte = GetPixelsPerByte(baseImage.Bpp);
            int tileSizeInBytes = TILESIZEINPIXELS / pixelsPerByte;

            int subImageWidthInBytes = subImageWidth / pixelsPerByte;
            int baseImageWidthInBytes = baseImage.Width / pixelsPerByte;
            int subImageSizeInBytes = subImageWidth * subImageHeight / pixelsPerByte;

            int tilesPerRow = baseImageWidthInBytes / tileSizeInBytes;

            int tileColumn = startTileIndex % tilesPerRow;
            int tileRow = startTileIndex / tilesPerRow;

            int xTileIndex = tileColumn * tileSizeInBytes;
            int yTileIndex = tileRow * TILESIZEINPIXELS;

            return new DigSubimageParams {
                SubImageSizeInBytes = subImageSizeInBytes,
                SubImageWidthInBytes = subImageWidthInBytes,
                BaseImageWidthInBytes = baseImageWidthInBytes,
                XTileIndex = xTileIndex,
                YTileIndex = yTileIndex,
            };
        }
    }
}
