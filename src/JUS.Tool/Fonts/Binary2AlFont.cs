using System.Buffers.Binary;
using System.Drawing;
using System.Text;
using JUS.Tool.Framework;
using Texim.Colors;
using Texim.Fonts;
using Texim.Images;
using Texim.Palettes;
using Texim.Pixels;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUS.Tool.Fonts;

/// <summary>
/// Convert the binary representation of an <see cref="AlFont"/> into the model.
/// </summary>
public class Binary2AlFont : IConverter<IBinary, AlFont>
{
    private static readonly Encoding CharEncoding = Encoding.GetEncoding("shift-jis");
    internal const int BorderSize = 1;

    /// <inheritdoc />
    public AlFont Convert(IBinary source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var reader = new DataReader(source.Stream);
        reader.Stream.Position = 0;
        var font = new AlFont();

        // Header
        FormatException.ThrowIfNotEqual(reader.ReadString(4), "ALFT");
        FormatException.ThrowIfNotEqual(reader.ReadByte(), 0); // version
        font.Features = (AlFontFeatures)reader.ReadByte();

        // Table info
        FormatException.ThrowIfNotEqual(reader.ReadString(2), "TB");
        int cellWidth = reader.ReadByte();
        int cellHeight = reader.ReadByte();

        // the binary includes the size of the border in grid mode
        font.BoundingBox = font.Features.HasFlag(AlFontFeatures.ImageGrid)
            ? new Size(cellWidth - (BorderSize * 2), cellHeight - (BorderSize * 2))
            : new Size(cellWidth, cellHeight);
        font.LineGap = font.BoundingBox.Height;
        font.GlyphsPerRow = reader.ReadUInt16();
        ushort groupCount = reader.ReadUInt16();

        Span<byte> sjisEncoded = stackalloc byte[2];
        for (int i = 0; i < groupCount; i++) {
            ushort startEncodedChar = reader.ReadUInt16();
            ushort endEncodedChar = reader.ReadUInt16();
            ushort glyphStart = reader.ReadUInt16();
            font.Groups.Add(new AlFontGlyphGroup {
                StartGlyph = startEncodedChar,
                EndGlyph = endEncodedChar,
                StartImageIndex = glyphStart,
            });

            for (ushort g = startEncodedChar; g <= endEncodedChar; g++) {
                BinaryPrimitives.WriteUInt16LittleEndian(sjisEncoded, g);
                string glyphChar = CharEncoding.GetString(sjisEncoded);
                int codepoint = char.ConvertToUtf32(glyphChar, 0);

                var glyph = new IndexedGlyph {
                    Index = glyphStart + (g - startEncodedChar),
                    CodePoint = codepoint,
                    BearingX = 0, // no info
                    AdvanceWidth = font.BoundingBox.Width, // no variable width font
                };
                font.Glyphs.Add(glyph);
            }
        }

        // Glyphs are unsorted in file, it's nice to view them sorted as they appear in the image.
        font.Glyphs.Sort((a, b) => a.Index.CompareTo(b.Index));

        reader.Stream.SkipPadding(4);

        string fontImageFormat = reader.ReadString(2);
        IndexedPaletteImage fontImage = fontImageFormat switch {
            "BM" => ReadBitmap(reader, font),
            "DS" => ReadDsig(reader),
            _ =>throw new FormatException("Unsupported font image"),
        };

        font.Palettes = new PaletteCollection(fontImage.Palettes);
        font.ColorsCount = fontImage.Palettes[0].Colors.Count;

        // "Cut" from the bitmap image each glyph (don't forget to include border in grid mode)
        int borderCellOffset = font.Features.HasFlag(AlFontFeatures.ImageGrid) ? BorderSize : 0;
        font.BitmapDimension = new Size(fontImage.Width, fontImage.Height);
        foreach (IIndexedGlyph glyph in font.Glyphs) {
            var glyphCell = new Rectangle(
                ((glyph.Index % font.GlyphsPerRow) * cellWidth) + borderCellOffset,
                ((glyph.Index / font.GlyphsPerRow) * cellHeight) + borderCellOffset,
                font.BoundingBox.Width,
                font.BoundingBox.Height);
            glyph.Image = fontImage.SubImage(glyphCell);
        }

        return font;
    }

    private static IndexedPaletteImage ReadBitmap(DataReader reader, AlFont font)
    {
        // It's just a standard BMP format...
        long bmpOffset = reader.Stream.Position - 2;

        // Bitmap file header
        _ = reader.ReadUInt32(); // total size
        FormatException.ThrowIfNotEqual(reader.ReadUInt16(), 0); // reserved
        FormatException.ThrowIfNotEqual(reader.ReadUInt16(), 0); // reserved
        uint bitmapOffset = reader.ReadUInt32();

        // DIB header 'BITMAPINFOHEADER' type
        FormatException.ThrowIfNotEqual(reader.ReadUInt32(), 40u); // section length
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        FormatException.ThrowIfNotEqual(reader.ReadUInt16(), 1u); // color planes count
        ushort bpp = reader.ReadUInt16(); // game supports 1bpp, 4bpp, and 8bpp
        FormatException.ThrowIfNotEqual(reader.ReadUInt32(), 0u); // compression method (BI_RGB none)
        _ = reader.ReadUInt32(); // + 2 for some (bug) reason
        font.BitmapResolution = new Size(reader.ReadInt32(), reader.ReadInt32());
        int paletteColorCount = reader.ReadInt32(); // hard-code in-game to bpp
        FormatException.ThrowIfNotEqual(reader.ReadUInt32(), 0u); // important color count

        // Color table
        Rgb[] colors = reader.ReadColors<Rgb32Encoding>(paletteColorCount);

        // As there is only one font, and it's 1bpp, we implement that mode
        // Note that the BMP format has the row inverted
        reader.Stream.Position = bmpOffset + bitmapOffset;
        if (bpp != 1) {
            throw new FormatException("This tool only support 1bpp");
        }

        IndexedPixel[] pixels = reader.ReadPixels<Indexed1BppMSbFirstEncoding>(width * height);
        pixels.FlipVertical(new Size(width, height));

        return new IndexedPaletteImage {
            Width = width,
            Height = height,
            Pixels = pixels,
            Palettes = [new Palette(colors)],
        };
    }

    private static IndexedPaletteImage ReadDsig(DataReader reader)
    {
        FormatException.ThrowIfNotEqual(reader.ReadString(2), "IG"); // full header DSIG
        FormatException.ThrowIfNotEqual(reader.ReadByte(), 1); // version
        byte paletteLength = reader.ReadByte();
        ushort paletteCount = reader.ReadUInt16();
        ushort width = reader.ReadUInt16();
        ushort height = reader.ReadUInt16();

        PaletteCollection palettes = new();
        for (int i = 0; i < paletteCount; i++) {
            Rgb[] colors = reader.ReadColors<Bgr555Encoding>(paletteLength / 2);
            palettes.Palettes.Add(new Palette(colors));
        }

        IndexedPixel[] pixels = reader.ReadPixels<Indexed4BppEncoding>(width * height);
        return new IndexedPaletteImage {
            Width = width,
            Height = height,
            Pixels = pixels,
            Palettes = palettes.Palettes,
        };
    }
}
