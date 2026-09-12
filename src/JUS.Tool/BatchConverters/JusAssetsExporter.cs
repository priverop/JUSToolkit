using JUS.Tool.BatchConverters.Strategies;
using JUS.Tool.Utils;
using Microsoft.Extensions.Logging;
using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace JUS.Tool.BatchConverters;

/// <summary>
/// Converter that creates a new container with the all the game assets ready to export for editing.
/// </summary>
/// <remarks>This container does not modify the input (it doesn't transform any of its nodes).</remarks>
public class JusAssetsExporter : IConverter<NodeContainerFormat, NodeContainerFormat>
{
    private readonly ILogger<JusAssetsExporter> logger = JusLoggerFactory.Instance.CreateLogger<JusAssetsExporter>();
    private readonly string? languageCode;
    private readonly IAssetExportStrategy[] strategies;

    /// <summary>
    /// Initializes a new instance of the <see cref="JusAssetsExporter"/> class.
    /// </summary>
    /// <param name="languageCode">The target translation language, or null to generate templates.</param>
    public JusAssetsExporter(string? languageCode)
    {
        this.languageCode = languageCode;
        bool createTemplate = string.IsNullOrEmpty(languageCode);
        strategies = [
            new DeckExportStrategy(createTemplate),
            new InfoDeckExportStrategy(createTemplate),
            new FontExportStrategy(),
        ];
    }

    /// <inheritdoc />
    public NodeContainerFormat Convert(NodeContainerFormat source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var texts = new Node(languageCode ?? "templates");
        var fonts = new Node("fonts");
        var images = new Node("images");

        foreach (Node asset in Navigator.IterateNodes(source.Root)) {
            logger.LogTrace("Exporting asset: {Path}", asset.Path);
            IAssetExportStrategy? strategy = strategies.FirstOrDefault(s => s.CanExport(asset));
            if (strategy is null) {
                logger.LogInformation("Non-exportable asset: {Path}", asset.Path);
                continue;
            }

            Node output = strategy switch {
                DeckExportStrategy or InfoDeckExportStrategy => texts,
                FontExportStrategy => fonts,
                _ => throw new NotSupportedException(),
            };
            IEnumerable<Node> exported = strategy.Export(asset);
            output.Add(exported);
        }

        return new NodeContainerFormat([texts, fonts, images]);
    }
}
