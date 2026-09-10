using System.Drawing;
using Texim.Colors;
using Texim.Fonts;
using Texim.Images;
using Texim.Palettes;
using Texim.Pixels;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUS.Tool.Fonts;

/// <summary>
/// Convert an <see cref="AlFont"/> font format into its binary representation.
/// </summary>
public class AlFont2Binary : IConverter<AlFont, BinaryFormat>
{
    /// <inheritdoc />
    public BinaryFormat Convert(AlFont source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var binary = new BinaryFormat();
        var writer = new DataWriter(binary.Stream);

        // Header
        writer.Write("ALFT".ToCharArray());
        writer.Write((byte)0);
        writer.Write((byte)source.Features);

        // Table info
        int cellWidth = source.Features.HasFlag(AlFontFeatures.ImageGrid)
            ? source.BoundingBox.Width + (Binary2AlFont.BorderSize * 2)
            : source.BoundingBox.Width;
        int cellHeight = source.Features.HasFlag(AlFontFeatures.ImageGrid)
            ? source.BoundingBox.Height + (Binary2AlFont.BorderSize * 2)
            : source.BoundingBox.Height;
        writer.Write("TB".ToCharArray());
        writer.Write((byte)cellWidth);
        writer.Write((byte)cellHeight);
        writer.Write((ushort)source.GlyphsPerRow);
        writer.Write((ushort)source.Groups.Count);

        foreach (AlFontGlyphGroup group in source.Groups) {
            writer.Write(group.StartGlyph);
            writer.Write(group.EndGlyph);
            writer.Write((ushort)group.StartImageIndex);
        }

        writer.Stream.WritePadding(0x00, 4);

        // Create the image as both sub-formats are bitmaps
        var fontImage = new IndexedImage(source.BitmapDimension.Width, source.BitmapDimension.Height);

        if (source.Features.HasFlag(AlFontFeatures.ImageGrid)) {
            // BM are a bit more complex because they have a "grid" (a border around glyphs),
            // background with second color, and empty (missing) glyphs still have their cell drawn.
            Array.Fill(fontImage.Pixels, new IndexedPixel(1));
            int lastGlyphIndex = source.Glyphs
                .Select(x => x.Index)
                .Max()
                .Pad(source.GlyphsPerRow);
            for (int i = 0; i < lastGlyphIndex; i++) {
                int x = (i % source.GlyphsPerRow) * cellWidth;
                int y = (i / source.GlyphsPerRow) * cellHeight;
                var glyphGrid = new Rectangle(x, y, cellWidth, cellHeight);
                DrawRectangle(fontImage, glyphGrid, Binary2AlFont.BorderSize, 0);
            }
        }

        int borderCellOffset = source.Features.HasFlag(AlFontFeatures.ImageGrid) ? Binary2AlFont.BorderSize : 0;
        foreach (IIndexedGlyph glyph in source.Glyphs) {
            if (glyph.Image is null) {
                continue;
            }

            int x = (glyph.Index % source.GlyphsPerRow) * cellWidth;
            int y = (glyph.Index / source.GlyphsPerRow) * cellHeight;
            fontImage.Paste(glyph.Image, new Point(x + borderCellOffset, y + borderCellOffset));
        }

        if (source.Features.HasFlag(AlFontFeatures.DsigImage)) {
            WriteDsig(writer, source, fontImage);
        } else {
            WriteBitmap(writer, source, fontImage);
        }

        return binary;
    }

    private static void WriteBitmap(DataWriter writer, AlFont font, IndexedImage fontImage)
    {
        long bmpOffset = writer.Stream.Position;
        IPalette palette = font.Palettes.Palettes.FirstOrDefault()
            ?? throw new FormatException("Missing font palette");

        fontImage.Pixels.FlipVertical(new Size(fontImage.Width, fontImage.Height));
        byte[] encodedPixels = Indexed1BppMSbFirstEncoding.Instance.Encode(fontImage.Pixels);

        // Bitmap file header
        writer.Write("BM".ToCharArray());
        writer.Write(0); // total size placeholder
        writer.Write((ushort)0); // reserved
        writer.Write((ushort)0); // reserved
        writer.Write(0); // pixel offset

        // DIB header 'BITMAPINFOHEADER' type
        writer.Write(40u); // section size
        writer.Write(fontImage.Width);
        writer.Write(fontImage.Height);
        writer.Write((ushort)1); // color planes
        writer.Write((ushort)1); // we only support 1bpp
        writer.Write(0); // compression method (none)
        writer.Write(encodedPixels.Length + 2); // +2 is a game bug
        writer.Write(font.BitmapResolution.Width);
        writer.Write(font.BitmapResolution.Height);
        writer.Write(palette.Colors.Count);
        writer.Write(0); // important colors - ignored

        // Color table
        writer.Write<Rgb32Encoding>(palette.Colors);

        long pixelOffset = writer.Stream.Position - bmpOffset;
        using (writer.Stream.EnterWithPosition(bmpOffset + 0xA)) {
            writer.Write((uint)pixelOffset);
        }

        writer.Write(encodedPixels);

        long sectionLength = writer.Stream.Position - bmpOffset;
        using (writer.Stream.EnterWithPosition(bmpOffset + 2)) {
            writer.Write((uint)sectionLength);
        }
    }

    private static void WriteDsig(DataWriter writer, AlFont font, IndexedImage fontImage)
    {
        writer.Write("DSIG".ToCharArray());
        writer.Write((byte)1); // version
        writer.Write((byte)((font.Palettes.Palettes.FirstOrDefault()?.Colors.Count ?? 0) * 2));
        writer.Write((ushort)font.Palettes.Palettes.Count);
        writer.Write((ushort)fontImage.Width);
        writer.Write((ushort)fontImage.Height);

        foreach (IPalette palette in font.Palettes.Palettes) {
            writer.Write<Bgr555Encoding>(palette.Colors);
        }

        writer.Write<Indexed4BppEncoding>(fontImage.Pixels);
    }

    private static void DrawRectangle(IIndexedImage image, Rectangle rect, int borderThickness, short colorIdx)
    {
        // Top left -> right
        FillRectangle(image, rect.X, rect.Y, rect.Width + (borderThickness * 2), borderThickness, colorIdx);

        // Left top -> bottom
        FillRectangle(image, rect.X, rect.Y, borderThickness, rect.Height + (borderThickness * 2), colorIdx);

        // Bottom left -> right
        FillRectangle(image, rect.X, rect.Y + rect.Height + borderThickness, rect.Width + (borderThickness * 2), borderThickness, colorIdx);

        // Right top -> bottom
        FillRectangle(image, rect.X + rect.Width + borderThickness, rect.Y, borderThickness, rect.Height + (borderThickness * 2), colorIdx);
    }

    private static void FillRectangle(IIndexedImage image, int x, int y, int width, int height, short colorIdx)
    {
        for (int w = 0; w < width; w++) {
            for (int h = 0; h < height; h++) {
                int imageIdx = ((y + h) * image.Width) + x + w;
                image.Pixels[imageIdx] = new IndexedPixel(colorIdx);
            }
        }
    }
}
