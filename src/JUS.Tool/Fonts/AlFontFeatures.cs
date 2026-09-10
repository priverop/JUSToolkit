namespace JUS.Tool.Fonts;

/// <summary>
/// Supported features of an ALFT font.
/// </summary>
[Flags]
public enum AlFontFeatures
{
    /// <summary>
    /// None.
    /// </summary>
    None = 0,

    /// <summary>
    /// The font image has a drawn grid.
    /// </summary>
    ImageGrid = 1 << 0,

    /// <summary>
    /// The font image is a DSIG indexed graphics. Otherwise, it's a BMP.
    /// </summary>
    DsigImage = 1 << 1,

    /// <summary>
    /// The font only supports ASCII characters.
    /// </summary>
    AsciiOnly = 1 << 2,
}
