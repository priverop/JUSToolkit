using System;
using Texim.Images;
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
    public static class DigExtension
    {
        public static IndexedImage SubImages(this IIndexedImage image, int startX, int startY, int width, int height)
        {
            var subImage = new IndexedImage(width, height);
            CopySubImage(image.Pixels, subImage.Pixels, image.Width, startX, startY, width, height);
            return subImage;
        }


        public static void CopySubImage<T>(T[] source, T[] destination, int sourceWidth, int startX, int startY, int width, int height)
        {
            int idx = 0;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    int fullIndex = ((startY + y) * sourceWidth) + startX + x;
                    destination[idx++] = source[fullIndex];
                }
            }
        }
    }
}
