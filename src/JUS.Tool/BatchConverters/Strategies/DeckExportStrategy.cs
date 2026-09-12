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
/// Strategy to export Deck container.
/// </summary>
/// <param name="createTemplate">Value indicating whether to create PO templates or PO.</param>
public class DeckExportStrategy(bool createTemplate) : IAssetExportStrategy
{
    private readonly ILogger<DeckExportStrategy> logger = JusLoggerFactory.Instance.CreateLogger<DeckExportStrategy>();
    private readonly string extension = createTemplate ? ".pot" : ".po";

    /// <inheritdoc />
    public AssetFormatKind ExportFormat => AssetFormatKind.Text;

    /// <inheritdoc />
    public bool CanExport(Node asset) => asset.Path.EndsWith("data/deck/Deck.aar");

    /// <inheritdoc />
    public IEnumerable<Node> Export(Node asset)
    {
        logger.LogDebug("Reading decks");
        Node exportedDecks = new("deck");

        using NodeContainerFormat container = asset.GetFormatAs<IBinary>()
            .ConvertWith(new Binary2Alar());
        Node containerRoot = container.Root.Children["deck"];

        // Pack the read decks into a container, so we export one .po per parent and type
        foreach (Node parent in containerRoot.Children) {
            logger.LogDebug("Exporting deck container {Name}", parent.Name);
            NodeContainerFormat deckContainer = new();
            NodeContainerFormat pDeckContainer = new();

            foreach (Node node in parent.Children) {
                // Use PDeck or Deck converters and ignore empty files.
                if (node.Name[0] == 'p') {
                    PDeck pdeck = new Binary2PDeck().Convert(node.GetFormatAs<IBinary>());
                    if (pdeck.Name.Length > 0) {
                        pDeckContainer.Root.Add(new Node(node.Name, pdeck));
                    } else {
                        logger.LogDebug("Ignoring empty PDeck: {Path}", node.Path);
                    }
                } else {
                    Deck deck = new Binary2Deck().Convert(node.GetFormatAs<IBinary>());
                    if (deck.Name.Length > 0) {
                        deckContainer.Root.Add(new Node(node.Name, deck));
                    } else {
                        logger.LogDebug("Ignoring empty Deck: {Path}", node.Path);
                    }
                }
            }

            if (deckContainer.Root.Children.Count > 0) {
                Node exportedDeck = new Node($"deck-{parent.Name}{extension}", deckContainer)
                    .TransformWith(new Deck2Po())
                    .TransformWith(new Po2Binary());
                exportedDecks.Add(exportedDeck);
            }
            if (pDeckContainer.Root.Children.Count > 0) {
                Node exportedPDeck = new Node($"deck-{parent.Name}_p{extension}", pDeckContainer)
                    .TransformWith(new PDeck2Po())
                    .TransformWith(new Po2Binary());
                exportedDecks.Add(exportedPDeck);
            }
        }

        return [exportedDecks];
    }
}
