using Texim.Colors;
using Texim.Images;
using Texim.Sprites;

namespace JUS.Tool.Graphics
{
    /// <summary>
    /// Draws the borders of the segments of a sprite on top of an image.
    /// </summary>
    public static class SegmentDrawer
    {
        private const int BorderThickness = 1;

        private static readonly Rgb[] Colors = [
            new Rgb(0xFF, 0x00, 0x00),
            new Rgb(0x00, 0xFF, 0x4A),
            new Rgb(0x94, 0x00, 0xFF),
            new Rgb(0xFF, 0xDF, 0x00),
            new Rgb(0x00, 0xD4, 0xFF),
            new Rgb(0xFF, 0x00, 0x89),
            new Rgb(0x3F, 0xFF, 0x00),
            new Rgb(0x0A, 0x00, 0xFF),
            new Rgb(0xFF, 0x55, 0x00),
            new Rgb(0x00, 0xFF, 0x9F),
            new Rgb(0xEA, 0x00, 0xFF),
            new Rgb(0xC9, 0xFF, 0x00),
            new Rgb(0x00, 0x7F, 0xFF),
            new Rgb(0xFF, 0x00, 0x34),
            new Rgb(0x00, 0xFF, 0x15),
            new Rgb(0x60, 0x00, 0xFF),
            new Rgb(0xFF, 0xAA, 0x00),
            new Rgb(0x00, 0xFF, 0xF4),
            new Rgb(0xFF, 0x00, 0xBE),
            new Rgb(0x74, 0xFF, 0x00),
            new Rgb(0x00, 0x29, 0xFF),
            new Rgb(0xFF, 0x20, 0x00),
            new Rgb(0x00, 0xFF, 0x6A),
            new Rgb(0xB5, 0x00, 0xFF),
            new Rgb(0xFE, 0xFF, 0x00),
            new Rgb(0x00, 0xB3, 0xFF),
            new Rgb(0xFF, 0x00, 0x69),
            new Rgb(0x1E, 0xFF, 0x00),
            new Rgb(0x2B, 0x00, 0xFF),
            new Rgb(0xFF, 0x75, 0x00),
            new Rgb(0x00, 0xFF, 0xC0),
            new Rgb(0xFF, 0x00, 0xF3),
            new Rgb(0xA8, 0xFF, 0x00),
            new Rgb(0x00, 0x5E, 0xFF),
            new Rgb(0xFF, 0x00, 0x14),
            new Rgb(0x00, 0xFF, 0x36),
            new Rgb(0x80, 0x00, 0xFF),
            new Rgb(0xFF, 0xCB, 0x00),
            new Rgb(0x00, 0xE8, 0xFF),
            new Rgb(0xFF, 0x00, 0x9E),
            new Rgb(0x53, 0xFF, 0x00),
            new Rgb(0x00, 0x09, 0xFF),
            new Rgb(0xFF, 0x41, 0x00),
            new Rgb(0x00, 0xFF, 0x8B),
            new Rgb(0xD5, 0x00, 0xFF),
            new Rgb(0xDD, 0xFF, 0x00),
            new Rgb(0x00, 0x93, 0xFF),
            new Rgb(0xFF, 0x00, 0x48),
            new Rgb(0x00, 0xFF, 0x01),
            new Rgb(0x4C, 0x00, 0xFF),
            new Rgb(0xFF, 0x96, 0x00),
            new Rgb(0x00, 0xFF, 0xE0),
            new Rgb(0xFF, 0x00, 0xD2),
            new Rgb(0x88, 0xFF, 0x00),
            new Rgb(0x00, 0x3D, 0xFF),
            new Rgb(0xFF, 0x0C, 0x00),
            new Rgb(0x00, 0xFF, 0x56),
            new Rgb(0xA1, 0x00, 0xFF),
            new Rgb(0xFF, 0xEB, 0x00),
            new Rgb(0x00, 0xC7, 0xFF),
            new Rgb(0xFF, 0x00, 0x7D),
            new Rgb(0x33, 0xFF, 0x00),
            new Rgb(0x17, 0x00, 0xFF),
            new Rgb(0xFF, 0x61, 0x00),
            new Rgb(0x00, 0xFF, 0xAC),
            new Rgb(0xF6, 0x00, 0xFF),
            new Rgb(0xBD, 0xFF, 0x00),
            new Rgb(0x00, 0x72, 0xFF),
            new Rgb(0xFF, 0x00, 0x28),
            new Rgb(0x00, 0xFF, 0x22),
            new Rgb(0x6C, 0x00, 0xFF),
            new Rgb(0xFF, 0xB7, 0x00),
            new Rgb(0x00, 0xFC, 0xFF),
            new Rgb(0xFF, 0x00, 0xB2),
            new Rgb(0x67, 0xFF, 0x00),
            new Rgb(0x00, 0x1D, 0xFF),
            new Rgb(0xFF, 0x2D, 0x00),
            new Rgb(0x00, 0xFF, 0x77),
            new Rgb(0xC1, 0x00, 0xFF),
            new Rgb(0xF1, 0xFF, 0x00),
            new Rgb(0x00, 0xA7, 0xFF),
            new Rgb(0xFF, 0x00, 0x5C),
            new Rgb(0x12, 0xFF, 0x00),
            new Rgb(0x37, 0x00, 0xFF),
            new Rgb(0xFF, 0x82, 0x00),
            new Rgb(0x00, 0xFF, 0xCC),
            new Rgb(0xFF, 0x00, 0xE6),
            new Rgb(0x9C, 0xFF, 0x00),
            new Rgb(0x00, 0x52, 0xFF),
            new Rgb(0xFF, 0x00, 0x07),
            new Rgb(0x00, 0xFF, 0x42),
            new Rgb(0x8D, 0x00, 0xFF),
            new Rgb(0xFF, 0xD7, 0x00),
            new Rgb(0x00, 0xDB, 0xFF),
            new Rgb(0xFF, 0x00, 0x91),
            new Rgb(0x47, 0xFF, 0x00),
            new Rgb(0x03, 0x00, 0xFF),
            new Rgb(0xFF, 0x4D, 0x00),
            new Rgb(0x00, 0xFF, 0x98),
            new Rgb(0xE2, 0x00, 0xFF),
        ];

        /// <summary>
        /// Gets a color from the list.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <returns>The color.</returns>
        public static Rgb GetColor(int index) => Colors[index % Colors.Length];

        /// <summary>
        /// Draw the border of a segment on top of the image.
        /// </summary>
        /// <param name="image">Image to draw on.</param>
        /// <param name="segment">Segment to draw.</param>
        /// <param name="color">Color of the border.</param>
        public static void DrawSegment(RgbImage image, IImageSegment segment, Rgb color)
        {
            ArgumentNullException.ThrowIfNull(image);
            ArgumentNullException.ThrowIfNull(segment);

            (int x, int y) = Dig.GetTilePosition(segment.TileIndex, image.Width);

            // The border is drawn outside the segment.
            DrawRectangle(image, x - BorderThickness, y - BorderThickness, segment.Width, segment.Height, color);
        }

        private static void DrawRectangle(RgbImage image, int x, int y, int width, int height, Rgb color)
        {
            // Top left -> right
            FillRectangle(image, x, y, width + (BorderThickness * 2), BorderThickness, color);

            // Left top -> bottom
            FillRectangle(image, x, y, BorderThickness, height + (BorderThickness * 2), color);

            // Bottom left -> right
            FillRectangle(image, x, y + height + BorderThickness, width + (BorderThickness * 2), BorderThickness, color);

            // Right top -> bottom
            FillRectangle(image, x + width + BorderThickness, y, BorderThickness, height + (BorderThickness * 2), color);
        }

        private static void FillRectangle(RgbImage image, int x, int y, int width, int height, Rgb color)
        {
            for (int w = 0; w < width; w++) {
                for (int h = 0; h < height; h++) {
                    int imageX = x + w;
                    int imageY = y + h;

                    if (imageX < 0 || imageY < 0 || imageX >= image.Width || imageY >= image.Height) {
                        continue;
                    }

                    image.Pixels[(imageY * image.Width) + imageX] = color;
                }
            }
        }
    }
}
