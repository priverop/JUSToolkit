using JUS.Tool.Utils;
using Microsoft.Extensions.Logging;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.BatchConverters.Strategies;

/// <summary>
/// Strategy to export menu images.
/// </summary>
/// <remarks>
/// This strategy applies to:
/// - Commu/commu_pack.aar
/// - database/database.aar
/// - input/input.aar
/// - JArena/JArena.aar
/// - jgalaxy/jgalaxy.aar
/// - jpower/jpower.aar
/// - option/option.aar
/// - ruleselect/ruleselect.aar
/// - topmenu/topmenu.aar
/// </remarks>
public class MenuImagesExportStrategy : IAssetExportStrategy
{
    private static readonly string[] ValidContainerPaths = [
        "/data/Commu/commu_pack.aar",
        "/data/database/database.aar",
        // "/data/deckcheck/deckcheck.aar",
        "/data/input/input.aar",
        "/data/JArena/JArena.aar",
        "/data/jgalaxy/jgalaxy.aar",
        "/data/jpower/jpower.aar",
        "/data/option/option.aar",
        "/data/ruleselect/ruleselect.aar",
        // "/data/stageselect/stageselect.aar",
        "/data/topmenu/topmenu.aar",
    ];

    private readonly ILogger<MenuImagesExportStrategy> logger = JusLoggerFactory.Instance.CreateLogger<MenuImagesExportStrategy>();

    /// <inheritdoc />
    public AssetFormatKind ExportFormat => AssetFormatKind.Image;

    /// <inheritdoc />
    public bool CanExport(Node asset) => ValidContainerPaths.Any(p => asset.Path.EndsWith(p));

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
