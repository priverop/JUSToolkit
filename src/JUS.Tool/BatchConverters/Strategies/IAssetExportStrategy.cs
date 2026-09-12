using Yarhl.FileSystem;

namespace JUS.Tool.BatchConverters.Strategies;

/// <summary>
/// Interface for strategies to export assets.
/// </summary>
public interface IAssetExportStrategy
{
    /// <summary>
    /// Get the exported format of the strategy.
    /// </summary>
    AssetFormatKind ExportFormat { get; }

    /// <summary>
    /// Determine if the asset can be exported with this strategy.
    /// </summary>
    /// <param name="asset">The asset to test.</param>
    /// <returns>Value indicating whether the asset can be exported.</returns>
    bool CanExport(Node asset);

    /// <summary>
    /// Export the asset into one or more editable nodes.
    /// </summary>
    /// <param name="asset">The input asset to convert.</param>
    /// <returns>The exportable nodes.</returns>
    /// <remarks>The input asset format does not change.</remarks>
    IEnumerable<Node> Export(Node asset);
}
