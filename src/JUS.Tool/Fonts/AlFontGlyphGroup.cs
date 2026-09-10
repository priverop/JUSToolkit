namespace JUS.Tool.Fonts;

/// <summary>
/// Represents the mapping of a group of glyphs in the font indexed image.
/// </summary>
public class AlFontGlyphGroup
{
    /// <summary>
    /// Gets or sets the first encoded glyph codepoint of the group.
    /// </summary>
    public ushort StartGlyph { get; set; }

    /// <summary>
    /// Gets or sets the last encoded glyph codepoint of the group.
    /// </summary>
    public ushort EndGlyph { get; set; }

    /// <summary>
    /// Gets or sets the index of the first glyph image in the group.
    /// </summary>
    public int StartImageIndex { get; set; }
}
