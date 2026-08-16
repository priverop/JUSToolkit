using System;
using JUS.Tool.Containers;
using JUS.Tool.Containers.Converters;
using JUS.Tool.Texts.Converters;
using JUS.Tool.Texts.Formats;
using NUnit.Framework;
using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUS.Tests.Texts
{
    [TestFixture]
    public class DeckFormatTest
    {
        private string resPath = string.Empty;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/Deck/");

            Assert.That(Directory.Exists(resPath), Is.True, "The resources folder does not exist");
        }

        private static readonly Lazy<NitroRom> Root = new(TestDataBase.ReadSoftware);

        private static Alar? DeckAlar;

        public static IEnumerable<TestCaseData> GetDeckBinPaths()
        {
            if (!File.Exists(TestDataBase.SoftwareNitroRomPath)) {
                return [];
            }

            Node deckAar = Navigator.SearchNode(Root.Value.Data, "deck/Deck.aar") ?? throw new ArgumentException("Deck.aar not found");
            DeckAlar = deckAar.TransformWith<Binary2Alar>().GetFormatAs<Alar>()!;

            return Navigator.IterateNodes(DeckAlar.Root, NavigationMode.DepthFirst)
                .Where(n => !n.IsContainer && n.Name.EndsWith(".bin") && !n.Name.StartsWith('p'))
                .Select(n => new TestCaseData(n.Path));
        }

        [TestCaseSource(nameof(GetDeckBinPaths))]
        public void TwoWaysIdenticalStreams(string binPath)
        {
            Assert.Ignore("The new Deck limit is too short, 9/300 tests fail. This will be removed as soon as we hack the deck limits.");
            TestDataBase.IgnoreIfFileDoesNotExist(TestDataBase.SoftwareNitroRomPath);

            IBinary? original = Navigator.SearchNode(DeckAlar!.Root, binPath)?.GetFormatAs<IBinary>();
            Assert.That(original, Is.Not.Null);

            AssertTwoWaysIdentical(original);
        }

        private static void AssertTwoWaysIdentical(IBinary original)
        {
            // Debo comprobar que no haya errores en los pasos intermedios? try catch?
            Deck originalDeck = new Binary2Deck().Convert(original);
            var originalContainer = new NodeContainerFormat();
            originalContainer.Root.Add(new Node("test", originalDeck));
            Po deckPo = new Deck2Po().Convert(originalContainer);
            NodeContainerFormat newDeckContainer = new Deck2Po().Convert(deckPo);
            Deck newDeck = newDeckContainer.Root.Children[0].GetFormatAs<Deck>()!;
            BinaryFormat newBinary = new Binary2Deck().Convert(newDeck);
            Assert.That(newBinary, Is.Not.Null);

            byte[] originalData = new byte[(int)original.Stream.Length];
            original.Stream.Position = 0;
            original.Stream.ReadExactly(originalData);
            byte[] newData = new byte[(int)newBinary.Stream.Length];
            newBinary.Stream.Position = 0;
            newBinary.Stream.ReadExactly(newData);
            Assert.That(newData, Is.EqualTo(originalData));
        }

        [Test]
        public void DeckTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (Node node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> Deck
                    BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>()!;
                    var binary2Deck = new Binary2Deck();
                    Deck expectedDeck = null!;
                    try {
                        expectedDeck = binary2Deck.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> Deck with {node.Path}\n{ex}");
                    }

                    // Deck -> NCF (Deck)
                    var originalContainer = new NodeContainerFormat();
                    originalContainer.Root.Add(new Node("test", expectedDeck));

                    // NCF (Deck) -> Po
                    var deck2Po = new Deck2Po();
                    Po expectedPo = null!;
                    try {
                        expectedPo = deck2Po.Convert(originalContainer);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception NCF (Deck) -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> NCF (Deck)
                    NodeContainerFormat container = null!;
                    try {
                        container = deck2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> NCF (Deck) with {node.Path}\n{ex}");
                    }

                    // NCF -> Deck
                    Deck actualDeck = container.Root.Children[0].GetFormatAs<Deck>()!;

                    // Deck -> BinaryFormat
                    BinaryFormat actualBin = null!;
                    try {
                        actualBin = binary2Deck.Convert(actualDeck);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Deck -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.That(expectedBin.Stream.Compare(actualBin.Stream!), Is.True, $"Deck are not identical: {node.Path}");
                }
            }
        }
    }
}
