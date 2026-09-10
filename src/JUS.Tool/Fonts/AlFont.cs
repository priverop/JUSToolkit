using System.Drawing;
using Texim.Fonts;
using Texim.Palettes;

namespace JUS.Tool.Fonts;

/// <summary>
/// Represents a bitmap font with format "ALFT".
/// </summary>
public class AlFont : BitmapFont
{
    /// <summary>
    /// Gets or sets the set of features the font supports.
    /// </summary>
    public AlFontFeatures Features { get; set; }

    /// <summary>
    /// Gets or sets the number of glyphs per row in the font image.
    /// </summary>
    public int GlyphsPerRow { get; set; }

    /// <summary>
    /// Gets or sets the mapping of glyphs in the indexed font image.
    /// </summary>
    public List<AlFontGlyphGroup> Groups { get; set; } = [];

    /// <summary>
    /// Gets or sets the font palette.
    /// </summary>
    public IPaletteCollection Palettes { get; set; } = new PaletteCollection();

    /// <summary>
    /// Gets or sets the size of the font image.
    /// </summary>
    public Size BitmapDimension { get; set; }

    /// <summary>
    /// Gets or sets the font image resolution.
    /// </summary>
    public Size BitmapResolution { get; set; }
}
