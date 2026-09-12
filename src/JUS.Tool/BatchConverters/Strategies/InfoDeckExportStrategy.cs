using JUS.Tool.Containers.Converters;
using JUS.Tool.Texts.Converters;
using JUS.Tool.Texts.Formats;
using JUS.Tool.Utils;
using Microsoft.Extensions.Logging;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUS.Tool.BatchConverters.Strategies;

/// <summary>
/// Strategy to export InfoDeck.aar container.
/// </summary>
/// <param name="createTemplate">Value indicating whether to create PO templates or PO.</param>
public class InfoDeckExportStrategy(bool createTemplate) : IAssetExportStrategy
{
    private readonly ILogger<InfoDeckExportStrategy> logger = JusLoggerFactory.Instance.CreateLogger<InfoDeckExportStrategy>();
    private readonly string extension = createTemplate ? ".pot" : ".po";

    /// <inheritdoc />
    public AssetFormatKind ExportFormat => AssetFormatKind.Text;

    /// <inheritdoc />
    public bool CanExport(Node asset) => asset.Path.EndsWith("data/bin/InfoDeck.aar");

    /// <inheritdoc />
    public IEnumerable<Node> Export(Node asset)
    {
        logger.LogDebug("Reading InfoDeck");
        using NodeContainerFormat container = asset.GetFormatAs<IBinary>()
            .ConvertWith(new Binary2Alar());

        var exportedDecks = new Node("InfoDeck");
        Node deck = container.Root.Children["bin"].Children["deck"];
        foreach (Node deckNode in deck.Children) {
            if (deckNode.Stream.Length == 0) {
                logger.LogTrace("Ignoring empty file: {Name}", deckNode.Name);
                continue;
            }

            logger.LogDebug("Exporting InfoDeck / deck: {Name}", deckNode.Name);
            BinaryTextFormat po = deckNode.GetFormatAs<IBinary>()
                .ConvertWith<IBinary, InfoDeckDeck>(new Binary2InfoDeckDeck())
                .ConvertWith<InfoDeckDeck, Po>(new InfoDeckDeck2Po())
                .ConvertWith(new Po2Binary());
            exportedDecks.Add(new Node($"bin-deck-{deckNode.Name}{extension}", po));
        }

        var exportedInfos = new Node("InfoDeck-Info");
        Node info = container.Root.Children["bin"].Children["info"];
        foreach (Node infoNode in info.Children) {
            if (infoNode.Stream.Length == 0) {
                logger.LogTrace("Ignoring empty file: {Name}", infoNode.Name);
                continue;
            }

            logger.LogDebug("Exporting InfoDeck / info: {Name}", infoNode.Name);
            BinaryTextFormat po = infoNode.GetFormatAs<IBinary>()
                .ConvertWith<IBinary, InfoDeckInfo>(new Binary2InfoDeckInfo())
                .ConvertWith<InfoDeckInfo, Po>(new InfoDeckInfo2Po())
                .ConvertWith(new Po2Binary());
            exportedInfos.Add(new Node($"bin-info-{infoNode.Name}{extension}", po));
        }

        return [exportedDecks, exportedInfos];
    }
}
