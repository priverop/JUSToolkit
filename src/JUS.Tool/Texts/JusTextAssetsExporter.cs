using JUS.Tool.Containers.Converters;
using JUS.Tool.Texts.Converters;
using JUS.Tool.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUS.Tool.Texts;

/// <summary>
/// Converter that creates a new container with the game text assets ready to export for editing.
/// </summary>
/// <remarks>This container does not modify the input (it doesn't transform any of its nodes).</remarks>
/// <param name="createTemplate">Value indicating whether to create a PO template or a target language specific PO.</param>
public class JusTextAssetsExporter(bool createTemplate): IConverter<NodeContainerFormat, NodeContainerFormat>
{
    private readonly string extension = createTemplate ? ".pot" : ".po";

    /// <inheritdoc />
    public NodeContainerFormat Convert(NodeContainerFormat source)
    {
        var decks = new Node("deck");
        decks.Add(ExportDecks(source.Root));

        return new NodeContainerFormat([decks]);
    }

    private IEnumerable<Node> ExportDecks(Node root)
    {
        using NodeContainerFormat container = root
            .Children["data"]
            .Children["deck"]
            .Children["Deck.aar"]
            .GetFormatAs<IBinary>()
            .ConvertWith(new Binary2Alar());
        Node containerRoot = container.Root.Children["deck"];

        // Pack the read decks into a container, so we export one .po per parent and type
        foreach (Node parent in containerRoot.Children) {
            NodeContainerFormat deckContainer = new();
            NodeContainerFormat pDeckContainer = new();

            foreach (Node deck in parent.Children.ToArray()) {
                // Use PDeck or Deck converters and ignore empty files.
                if (deck.Name[0] == 'p') {
                    deck.TransformWith(new Binary2PDeck());
                    if (deck.GetFormatAs<PDeck>().Name.Length > 0) {
                        pDeckContainer.Root.Add(deck);
                    }
                } else {
                    deck.TransformWith(new Binary2Deck());
                    if (deck.GetFormatAs<Deck>().Name.Length > 0) {
                        deckContainer.Root.Add(deck);
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
