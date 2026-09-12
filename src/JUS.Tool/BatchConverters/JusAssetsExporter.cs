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
            new JGalaxyTextExportStrategy(createTemplate),
            new JQuizTextExportStrategy(),
            new TutorialExportStrategy(createTemplate),
            new LooseTextsExportStrategy(createTemplate),
            new FontExportStrategy(),
            new ComicImagesExportStrategy(),
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
            IAssetExportStrategy[] matchingStrategies = strategies.Where(s => s.CanExport(asset)).ToArray();
            if (matchingStrategies.Length == 0) {
                logger.LogInformation("Non-exportable asset: {Path}", asset.Path);
                continue;
            }

            foreach (IAssetExportStrategy strategy in matchingStrategies) {
                logger.LogTrace("Exporting asset: {Path} with {Type}", asset.Path, strategy.GetType().Name);
                Node output = strategy.ExportFormat switch {
                    AssetFormatKind.Text => texts,
                    AssetFormatKind.Font => fonts,
                    AssetFormatKind.Image => images,
                    _ => throw new NotSupportedException(),
                };

                // Create a temporary node to hold the output, then move with merge strategy
                using Node exported = new("out");
                exported.Add(strategy.Export(asset));
                exported.GetFormatAs<NodeContainerFormat>().MoveChildrenTo(output, true);
            }
        }

        return new NodeContainerFormat([texts, fonts, images]);
    }
}
