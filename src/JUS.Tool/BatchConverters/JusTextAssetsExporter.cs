using JUS.Tool.Containers.Converters;
using JUS.Tool.Texts.Converters;
using JUS.Tool.Texts.Formats;
using JUS.Tool.Utils;
using Microsoft.Extensions.Logging;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUS.Tool.BatchConverters;

/// <summary>
/// Converter that creates a new container with the game text assets ready to export for editing.
/// </summary>
/// <remarks>This container does not modify the input (it doesn't transform any of its nodes).</remarks>
/// <param name="createTemplate">Value indicating whether to create a PO template or a target language specific PO.</param>
public class JusTextAssetsExporter(bool createTemplate): IConverter<NodeContainerFormat, NodeContainerFormat>
{
    private readonly ILogger<JusTextAssetsExporter> logger = JusLoggerFactory.Instance.CreateLogger<JusTextAssetsExporter>();
    private readonly string extension = createTemplate ? ".pot" : ".po";

    /// <inheritdoc />
    public NodeContainerFormat Convert(NodeContainerFormat source)
    {
        var decks = new Node("deck");
        decks.Add(ExportDecks(source.Root));

        IEnumerable<Node> infoDecks = ExportInfoDeck(source.Root);

        return new NodeContainerFormat([decks, ..infoDecks]);
    }

    private IEnumerable<Node> ExportInfoDeck(Node root)
    {
        logger.LogDebug("Reading InfoDeck");
        using NodeContainerFormat container = root
            .Children["data"]
            .Children["bin"]
            .Children["InfoDeck.aar"]
            .GetFormatAs<IBinary>()
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
            exportedDecks.Add(new Node($"bin-deck-{deckNode.Name}.po", po));
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
            exportedInfos.Add(new Node($"bin-info-{infoNode.Name}.po", po));
        }

        return [exportedDecks, exportedInfos];
    }

    private IEnumerable<Node> ExportDecks(Node root)
    {
        logger.LogDebug("Reading decks");
        using NodeContainerFormat container = root
            .Children["data"]
            .Children["deck"]
            .Children["Deck.aar"]
            .GetFormatAs<IBinary>()
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
                yield return new Node($"deck-{parent.Name}{extension}", deckContainer)
                    .TransformWith(new Deck2Po())
                    .TransformWith(new Po2Binary());
            }
            if (pDeckContainer.Root.Children.Count > 0) {
                yield return new Node($"deck-{parent.Name}_p{extension}", pDeckContainer)
                    .TransformWith(new PDeck2Po())
                    .TransformWith(new Po2Binary());
            }
        }
    }
}
