using JUS.Tool.Containers.Converters;
using JUS.Tool.Graphics.Converters;
using JUS.Tool.Utils;
using Microsoft.Extensions.Logging;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.BatchConverters.Strategies;

/// <summary>
/// Strategy to export sprites.
/// </summary>
/// <remarks>
/// This strategy applies to:
/// - battle/pause.aar
/// - Commu/commu_pack.aar
/// - Commu/error_2d.aar
/// - database/database.aar
/// - deckcheck/deckcheck.aar
/// - deckseelect/deckselect.aar
/// - demo/demo.aar
/// - ending/ending.aar
/// - info/info.aar
/// - input/input.aar
/// - JArena/JArena.aar
/// - jgalaxy/jgalaxy.aar
/// - jquiz/jquiz_pack.aar
/// - option/option.aar
/// - result/result.aar
/// - stageselect/stageselect.aar
/// - title/title.aar
/// </remarks>
public class SpritesExportStrategy : IAssetExportStrategy
{
    private readonly ILogger<SpritesExportStrategy> logger = JusLoggerFactory.Instance.CreateLogger<SpritesExportStrategy>();

    private static readonly string[] ValidContainerPaths = [
        "/data/battle/pause.aar",
        "/data/Commu/commu_pack.aar",
        "/data/Commu/error_2d.aar",
        "/data/database/database.aar",
        "/data/deckcheck/deckcheck.aar",
        "/data/deckselect/deckselect.aar",
        "/data/demo/demo.aar",
        "/data/ending/ending.aar",
        "/data/info/info.aar",
        "/data/input/input.aar",
        "/data/JArena/JArena.aar",
        "/data/jgalaxy/jgalaxy.aar",
        "/data/jquiz/jquiz_pack.aar",
        "/data/option/option.aar",
        "/data/result/result.aar",
        "/data/stageselect/stageselect.aar",
        "/data/title/title.aar",
    ];

    /// <inheritdoc />
    public AssetFormatKind ExportFormat => AssetFormatKind.Image;

    /// <inheritdoc />
    public bool CanExport(Node asset) => ValidContainerPaths.Any(p => asset.Path.EndsWith(p));

    /// <inheritdoc />
    public IEnumerable<Node> Export(Node asset)
    {
        logger.LogDebug("Reading sprites from: {Name}", asset.Name);

        // Unpack top container
        NodeContainerFormat container = new Binary2Alar().Convert(asset.GetFormatAs<IBinary>());

        // This iteration supports unpacking while processing the nodes
        Node exported = new("sprites");
        foreach (Node node in Navigator.IterateNodes(container.Root)) {
            if (node.Extension == ".aar") {
                node.TransformWith(new Binary2Alar());
            } else if (node.Extension == ".dtx") {
                Node[] exportedDstx = ExportDstx(node, asset.Name);
                exported.Add(exportedDstx);
            }
        }

        return [exported];
    }

    private Node[] ExportDstx(Node input, string containerName)
    {
        logger.LogDebug("Reading DSTX: {Name}", input.Name);
        try {
            Node exported = input
                .TransformWith<LzssDecompression>()
                .TransformWith<Dtx2Bitmaps>();

            foreach (Node spriteNode in exported.Children) {
                spriteNode.Name = $"{containerName}-{input.Name}-{spriteNode.Name}.png";
            }

            return exported.Children.ToArray();
        } catch (Exception ex) {
            logger.LogWarning(ex, "Cannot read DSTX: {ParentName}/{Name}", containerName, input.Name);
            return [];
        }
    }
}
