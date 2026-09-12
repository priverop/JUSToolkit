using JUS.Tool.Utils;
using Microsoft.Extensions.Logging;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.BatchConverters.Strategies;

/// <summary>
/// Strategy to export images from demo.aar container.
/// </summary>
public class ComicImagesExportStrategy : IAssetExportStrategy
{
    private static readonly Dictionary<string, int> DemoImages = new() {
        { "_03.dig", 0 },
        { "_05.dig", 1 },
        { "_07.dig", 2 },
        { "_09.dig", 3 },
    };

    private readonly ILogger<ComicImagesExportStrategy> logger = JusLoggerFactory.Instance.CreateLogger<ComicImagesExportStrategy>();

    /// <inheritdoc />
    public AssetFormatKind ExportFormat => AssetFormatKind.Image;

    /// <inheritdoc />
    public bool CanExport(Node asset) => asset.Path.EndsWith("/data/demo/demo.aar");

    /// <inheritdoc />
    public IEnumerable<Node> Export(Node asset)
    {
        logger.LogDebug("Reading demo (comic) images");
        NodeContainerFormat outputs = new Alar2Png(DemoImages).Convert(asset.GetFormatAs<IBinary>());
        foreach (Node node in outputs.Root.Children) {
            node.Name += ".png";
        }

        return [new Node("comics", outputs)];
    }
}
