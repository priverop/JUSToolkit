namespace JUS.Tool.Containers;

/// <summary>
/// Features of an ALAR container.
/// </summary>
[Flags]
public enum AlarFormatFeatures
{
    /// <summary>
    /// The container has no features.
    /// </summary>
    None = 0,

    /// <summary>
    /// The container provides the name of the children.
    /// </summary>
    Names = 1 << 0,

    /// <summary>
    /// Unknown feature, related to child file info bit 24.
    /// </summary>
    Unknown1 = 1 << 1,

    /// <summary>
    /// The container supports folders.
    /// </summary>
    Folders = 1 << 2,

    /// <summary>
    /// The child name hash use version 2 of the algorithm.
    /// </summary>
    PathHashV2 = 1 << 6,
}
