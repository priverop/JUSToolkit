namespace JUS.Tool.Containers;

/// <summary>
/// Represents a children in an ALAR container.
/// </summary>
public class AlarFileInfo
{
    /// <summary>
    /// Gets or sets the path of the file in the container with this metadata.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the internal game identifier of the file.
    /// </summary>
    public uint FileId { get; set; }

    /// <summary>
    /// Gets or sets flags about the file content.
    /// </summary>
    public uint Flags { get; set; }
}
