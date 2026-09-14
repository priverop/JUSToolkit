namespace JUS.Tool.BatchConverters.Strategies;

/// <summary>
/// Format category of assets.
/// </summary>
public enum AssetFormatKind
{
    /// <summary>
    /// Unknown format.
    /// </summary>
    Unknown,

    /// <summary>
    /// Binary format.
    /// </summary>
    Binary,

    /// <summary>
    /// Standard editable text format like PO.
    /// </summary>
    Text,

    /// <summary>
    /// Standard editable font format like PNG + YAML.
    /// </summary>
    Font,

    /// <summary>
    /// Standard editable graphic format like PNG or TIFF.
    /// </summary>
    Image
}
