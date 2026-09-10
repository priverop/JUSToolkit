using JUS.Tool.Containers.Converters;
using JUS.Tool.Texts.Converters;
using JUS.Tool.Texts.Formats;
using NUnit.Framework;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUS.Tests.Texts
{
    [TestFixture]
    public class DeckFormatTest
    {
        private static readonly Lazy<Node> DeckContainer = new(UnpackDeckContainer);

        // Ignored because of line length limitations, until we patch the assembly
        private static readonly string[] Ignored = [
            "jard/004.bin",
            "jard/023.bin",
            "jard/036.bin",
            "jarg/019.bin",
            "priv/002.bin",
            "priv/004.bin",
            "priv/008.bin",
            "smpl/003.bin"
        ];

        private static Node UnpackDeckContainer()
        {
            return TestDataBase.ReadSoftware()
                .Data
                .Children["deck"]
                .Children["Deck.aar"]
                .TransformWith(new Binary2Alar());
        }

        private static IEnumerable<TestCaseData> GetDeckNodes()
        {
            if (!File.Exists(TestDataBase.SoftwareNitroRomPath)) {
                return [];
            }

            return Navigator.IterateNodes(DeckContainer.Value, NavigationMode.DepthFirst)
                .Where(n => !n.IsContainer && n.Name[0] != 'p')
                .Select(n => new TestCaseData(n).SetArgDisplayNames(n.Path));
        }

        [TestCaseSource(nameof(GetDeckNodes))]
        public void PoRoundTripIsIdentical(Node node)
        {
            if (Ignored.Contains($"{node.Parent!.Name}/{node.Name}")) {
                Assert.Ignore("It won't be equal due to line length limitations");
            }

            // Binary -> Deck
            IBinary expectedBin = node.GetFormatAs<IBinary>();
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
            Deck actualDeck = container.Root.Children[0].GetFormatAs<Deck>();

            // Deck -> BinaryFormat
            BinaryFormat actualBin = binary2Deck.Convert(actualDeck);

            // Comparing Binaries
            bool areIdentical = expectedBin.Stream.Compare(actualBin.Stream);
            if (!areIdentical) {
                expectedBin.Stream.WriteTo($"expected_{node.Parent.Name}_{node.Name}.bin");
                actualBin.Stream.WriteTo($"actual_{node.Parent.Name}_{node.Name}.bin");
            }

            Assert.That(areIdentical, Is.True, "Decks are not identical");
        }
    }
}
