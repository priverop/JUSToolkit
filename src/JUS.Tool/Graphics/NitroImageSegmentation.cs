// Copyright (c) 2022 SceneGate

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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Texim.Colors;
using Texim.Images;
using Texim.Sprites;

namespace JUSToolkit.Graphics
{
    /// <summary>
    /// Image segmentation algorithm for Nintendo DS.
    /// </summary>
    public class NitroImageSegmentation : IImageSegmentation
    {
        private sealed record SplitSize(int TransparentLimit, int Size);

        // List of tries for width and height.
        // The algorithm search a size where from the limit border to the size border
        // there are non-transparent pixels. Otherwise, tries a smaller size to avoid
        // having large mostly transparent OAMs (the DS has a limit of # OAMs on memory).
        // The limits are so if half is transparent means it worth getting the next smaller size.
        private static readonly SplitSize[] SplitMode = new[] {
            new SplitSize(32, 64),
            new SplitSize(16, 32),
            new SplitSize(8, 16),
            new SplitSize(0, 8),
        };

        public int CanvasWidth { get; set; } = 256;

        public int CanvasHeight { get; set; } = 256;

        public bool SkipTrimming { get; set; }

        public SpriteRelativeCoordinatesKind RelativeCoordinates { get; set; } = SpriteRelativeCoordinatesKind.Center;

        public (Sprite Sprite, FullImage TrimmedImage) Segment(FullImage frame)
        {
            if (SearchNoTransparentPoint(frame, 0) == -1) {
                var emptySprite = new Sprite {
                    Width = 0,
                    Height = 0,
                    Segments = new Collection<IImageSegment>(),
                };
                var emptyImage = new FullImage(0, 0);
                return (emptySprite, emptyImage);
            }

            FullImage objImage;
            int startX = 0, startY = 0;
            if (SkipTrimming) {
                objImage = frame;
            } else {
                (startX, startY, objImage) = TrimImage(frame);
            }

            var segments = CreateObjects(objImage, startX, startY, 0, 0, objImage.Height);

            // Return new frame
            var sprite = new Sprite {
                Segments = new Collection<IImageSegment>(segments),
                Width = objImage.Width,
                Height = objImage.Height,
            };
            return (sprite, objImage);
        }

        private List<IImageSegment> CreateObjects(FullImage frame, int startX, int startY, int x, int y, int maxHeight)
        {
            var segments = new List<IImageSegment>();

            int firstNonTransX = SearchNoTransparentPoint(frame, 1, x, y, yEnd: y + maxHeight);
            int firstNonTransY = SearchNoTransparentPoint(frame, 0, x, y, yEnd: y + maxHeight);

            // Only transparent pixels at this point.
            if (firstNonTransX == -1 || firstNonTransY == -1) {
                return segments;
            }

            int newX = x, newY = y;

            if (!SkipTrimming) {
                // Go to first non-transparent pixel
                newX = firstNonTransX;
                newY = firstNonTransY;
            }

            int diffX = newX - x;
            diffX -= diffX % 8;
            x = diffX + x;

            int diffY = newY - y;
            diffY -= diffY % 8;
            y = diffY + y;

            // Reach the end of the image
            if (startX + x == frame.Width && startY + y == frame.Height) {
                return segments;
            }

            int width, height;
            bool foundValidSize;

            // If our cell is already valid, do not split further.
            if (IsValidSize(frame.Width - x, maxHeight - diffY)) {
                width = frame.Width - x;
                height = maxHeight - diffY;
                foundValidSize = true;
            } else {
                (width, height, foundValidSize) = GetObjectSize(frame, x, y, frame.Width, maxHeight - diffY);
            }

            if (foundValidSize) {
                var segment = new ImageSegment {
                    CoordinateX = startX + x,
                    CoordinateY = startY + y,
                    Width = width,
                    Height = height,
                    Layer = 0,
                };
                if (RelativeCoordinates == SpriteRelativeCoordinatesKind.Center) {
                    segment.CoordinateX -= CanvasWidth / 2;
                    segment.CoordinateY -= CanvasHeight / 2;
                }

                segments.Add(segment);
            }

            // Go to right
            if (frame.Width - (x + width) > 0) {
                segments.AddRange(CreateObjects(frame, startX, startY, x + width, y, height));
            }

            // Go to down
            int newMaxHeight = maxHeight - (height + diffY);
            if (newMaxHeight > 0) {
                segments.AddRange(CreateObjects(frame, startX, startY, x, y + height, newMaxHeight));
            }

            return segments;
        }

        private (int Width, int Height, bool IsValid) GetObjectSize(
            FullImage frame,
            int x,
            int y,
            int maxWidth,
            int maxHeight)
        {
            int minWidthConstraint = 0;
            int width = 0;
            int height = 0;

            // Try to get a valid object size
            // The problem is the width can get fixed to 64 and in that case the height can not be 8 or 16.
            // That's why we try to get a width and then a height, if non valid, try to find next smaller width.
            while (height == 0 && minWidthConstraint < SplitMode.Length) {
                // Get object width
                width = 0;
                for (int i = minWidthConstraint; i < SplitMode.Length && width == 0; i++) {
                    // If the potential segment is bigger than the remaining, check next size.
                    // Except if it's the latest (smaller).
                    if (i + 1 != SplitMode.Length && SplitMode[i].Size > maxWidth - x) {
                        continue;
                    }

                    // If it's not transparent from the limit to the end size, found it!
                    int nonTransparentRange = SplitMode[i].Size - SplitMode[i].TransparentLimit;
                    if (!IsTransparent(frame, x + SplitMode[i].TransparentLimit, nonTransparentRange, y, maxHeight)) {
                        width = SplitMode[i].Size;
                    }
                }

                // Everything is transparent, skip like it were a valid OAM (so we can split further)
                // We use the higher allowed width.
                if (width == 0) {
                    return (SplitMode[minWidthConstraint].Size, maxHeight, false);
                }

                // Get object height
                height = 0;
                for (int i = 0; i < SplitMode.Length && height == 0; i++) {
                    if (i + 1 != SplitMode.Length && SplitMode[i].Size > maxHeight) {
                        continue;
                    }

                    if (!IsValidSize(width, SplitMode[i].Size)) {
                        continue;
                    }

                    int nonTransparentRange = SplitMode[i].Size - SplitMode[i].TransparentLimit;
                    if (!IsTransparent(frame, x, width, y + SplitMode[i].TransparentLimit, nonTransparentRange)) {
                        height = SplitMode[i].Size;
                    }
                }

                minWidthConstraint++;
            }

            // This can happen if we can't trim the image to preserve layer positions.
            // In that case we can't find a valid OAM size starting at the left
            // that it's not 100% transparent. The smaller 8x8 at the left is transparent.
            // We report the smaller OAM to skip it and try at another pos.
            if (height == 0) {
                return (width, 8, false);
            }

            return (width, height, true);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.OrderingRules",
            "SA1204:Static elements should appear before instance elements",
            Justification = "Readability of the algorithm")]
        private static bool IsValidSize(int width, int height)
        {
            return (width, height) switch {
                // Square mode
                (8, 8) => true,
                (16, 16) => true,
                (32, 32) => true,
                (64, 64) => true,

                // Rectangle horizontal
                (16, 8) => true,
                (32, 8) => true,
                (32, 16) => true,
                (64, 32) => true,

                // Rectangle vertical
                (8, 16) => true,
                (8, 32) => true,
                (16, 32) => true,
                (32, 64) => true,

                _ => false,
            };
        }

        private static (int X, int Y, FullImage Trimmed) TrimImage(FullImage image)
        {
            // Get border points to get dimensions
            int xStart = SearchNoTransparentPoint(image, 1);
            int yStart = SearchNoTransparentPoint(image, 0);
            if (xStart == -1 && yStart == -1) {
                return (-1, -1, null);
            }

            int width = SearchNoTransparentPoint(image, 2) - xStart + 1;
            int height = SearchNoTransparentPoint(image, 3) - yStart + 1;

            // Size must be multiple of 8 due to Obj size
            if (width % 8 != 0) {
                width += 8 - (width % 8);
            }

            if (height % 8 != 0) {
                height += 8 - (height % 8);
            }

            if (xStart == -1) {
                return (0, 0, image);
            }

            var newPixels = new Rgb[width * height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    newPixels[(y * width) + x] = image.Pixels[((yStart + y) * image.Width) + xStart + x];
                }
            }

            var newImage = new FullImage(width, height) {
                Pixels = newPixels,
            };

            return (xStart, yStart, newImage);
        }

        private static int SearchNoTransparentPoint(
            FullImage image,
            int direction,
            int xStart = 0,
            int yStart = 0,
            int xEnd = -1,
            int yEnd = -1)
        {
            if (xEnd == -1) {
                xEnd = image.Width;
            }

            if (yEnd == -1) {
                yEnd = image.Height;
            }

            int point = -1;
            Rgb[] pixels = image.Pixels;
            int width = image.Width;
            bool stop = false;

            // Get top most
            if (direction == 0) {
                for (int y = yStart; y < yEnd && !stop; y++) {
                    for (int x = xStart; x < xEnd && !stop; x++) {
                        if (pixels[(y * width) + x].Alpha == 0) {
                            continue;
                        }

                        point = y;
                        stop = true;
                    }
                }

                // Get left most
            } else if (direction == 1) {
                for (int x = xStart; x < xEnd && !stop; x++) {
                    for (int y = yStart; y < yEnd && !stop; y++) {
                        if (pixels[(y * width) + x].Alpha == 0) {
                            continue;
                        }

                        point = x;
                        stop = true;
                    }
                }

                // Get right most
            } else if (direction == 2) {
                for (int x = xEnd - 1; x > 0 && !stop; x--) {
                    for (int y = yStart; y < yEnd && !stop; y++) {
                        if (pixels[(y * width) + x].Alpha == 0)
                            continue;

                        point = x;
                        stop = true;
                    }
                }

                // Get bottom most
            } else if (direction == 3) {
                for (int y = yEnd - 1; y > 0 && !stop; y--) {
                    for (int x = xStart; x < xEnd && !stop; x++) {
                        if (pixels[(y * width) + x].Alpha == 0)
                            continue;

                        point = y;
                        stop = true;
                    }
                }
            } else {
                throw new ArgumentOutOfRangeException(nameof(direction), "Only 0 to 3 values");
            }

            return point;
        }

        private static bool IsTransparent(
            FullImage image,
            int xStart,
            int xRange,
            int yStart,
            int yRange)
        {
            bool isTransparent = true;
            int xEnd = xStart + xRange > image.Width ? image.Width : xStart + xRange;
            int yEnd = yStart + yRange > image.Height ? image.Height : yStart + yRange;

            var pixels = image.Pixels;
            bool stop = false;
            for (int x = xStart; x < xEnd && !stop; x++) {
                for (int y = yStart; y < yEnd && !stop; y++) {
                    if (pixels[(y * image.Width) + x].Alpha != 0) {
                        isTransparent = false;
                        stop = true;
                    }
                }
            }

            return isTransparent;
        }
    }
}
