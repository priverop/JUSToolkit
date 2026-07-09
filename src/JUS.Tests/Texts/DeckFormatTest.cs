using JUS.Tool.Containers.Converters;
using JUS.Tool.Texts.Converters;
using JUS.Tool.Texts.Formats;
using NUnit.Framework;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUS.Tests.Texts
{
    public class DeckFormatTest
    {
        private static readonly Lazy<Node> DeckContainer = new(UnpackDeckContainer);

        private static Node UnpackDeckContainer()
        {
            return TestDataBase.ReadSoftware()
                .Data
                .Children["deck"]!
                .Children["Deck.aar"]!
                .TransformWith(new Binary2Alar());
        }

        private static IEnumerable<TestCaseData> GetDeckPaths()
        {
            if (!File.Exists(TestDataBase.SoftwareNitroRomPath)) {
                return [];
            }

            return Navigator.IterateNodes(DeckContainer.Value, NavigationMode.DepthFirst)
                .Where(n => !n.IsContainer && n.Name[0] != 'p')
                .Select(n => new TestCaseData(n.Path));
        }

        [TestCaseSource(nameof(GetDeckPaths))]
        public void PoRoundTripIsIdentical(string deckContainerPath)
        {
            Node? node = Navigator.SearchNode(DeckContainer.Value, deckContainerPath);
            Assert.That(node, Is.Not.Null);

            // Binary -> Deck
            IBinary expectedBin = node.GetFormatAs<IBinary>()!;
            var binary2Deck = new Binary2Deck();
            Deck expectedDeck = binary2Deck.Convert(expectedBin);

            if (expectedDeck.Name.Length == 0) {
                Assert.Pass("No text");
            }

            // Deck -> NCF (Deck)
            var originalContainer = new NodeContainerFormat();
            originalContainer.Root.Add(new Node("test", expectedDeck));

            // NCF (Deck) -> Po
            var deck2Po = new Deck2Po();
            Po expectedPo = deck2Po.Convert(originalContainer);

            // Po -> NCF (Deck)
            NodeContainerFormat container = deck2Po.Convert(expectedPo);

            // NCF -> Deck
            Deck actualDeck = container.Root.Children[0].GetFormatAs<Deck>()!;

            // Deck -> BinaryFormat
            BinaryFormat actualBin = binary2Deck.Convert(actualDeck);

            // Comparing Binaries
            Assert.That(
                expectedBin.Stream.Compare(actualBin.Stream),
                Is.True,
                $"Deck are not identical: {node.Path}");
        }
    }
}
