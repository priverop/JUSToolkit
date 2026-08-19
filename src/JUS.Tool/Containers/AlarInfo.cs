using System.Collections.ObjectModel;
using Yarhl.FileFormat;

namespace JUS.Tool.Containers;

/// <summary>
/// Represents the meta-information of an Alar container.
/// </summary>
public class AlarInfo : IFormat
{
    /// <summary>
    /// Gets or sets the ALAR format version.
    /// </summary>
    public byte Version { get; set; }

    /// <summary>
    /// Gets or sets the container feature flags.
    /// </summary>
    public AlarFormatFeatures Features { get; set; }

    /// <summary>
    /// Gets or sets the ID of the first file in the container.
    /// </summary>
    public uint FirstFileId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the last file in the container.
    /// </summary>
    public uint LastFileId { get; set; }

    /// <summary>
    /// Gets or sets the metadata of the container files.
    /// </summary>
    public Collection<AlarFileInfo> FilesMetadata { get; set; } = [];
}
