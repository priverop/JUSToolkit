using JUS.Tool.BatchConverters;
using JUS.Tool.BatchConverters.Strategies;
using JUS.Tool.Utils;
using Microsoft.Extensions.Logging;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tests.Batch;

/// <summary>
/// Strategy to export menu images.
/// </summary>
/// <remarks>This strategy applies to Commu/commu_pack.aar</remarks>
public class MenuImagesExportStrategy : IAssetExportStrategy
{
    private static readonly string[] validContainerPaths = [
        "/data/Commu/commu_pack.aar",
        "/data/database/database.aar",
        // "/data/deckcheck/deckcheck.aar",
        "/data/input/input.aar",
    ];

    private readonly ILogger<MenuImagesExportStrategy> logger = JusLoggerFactory.Instance.CreateLogger<MenuImagesExportStrategy>();

    /// <inheritdoc />
    public AssetFormatKind ExportFormat => AssetFormatKind.Image;

    /// <inheritdoc />
    public bool CanExport(Node asset) => validContainerPaths.Any(p => asset.Path.EndsWith(p));

    /// <inheritdoc />
    public IEnumerable<Node> Export(Node asset)
    {
        logger.LogDebug("Reading menu images from: {Name}", asset.Name);
        NodeContainerFormat outputs = new Alar2Png().Convert(asset.GetFormatAs<IBinary>());
        foreach (Node node in outputs.Root.Children) {
            node.Name = $"menu-{asset.Parent?.Name}-{node.Name}.png";
        }

        return [new Node("menus", outputs)];
    }
}
