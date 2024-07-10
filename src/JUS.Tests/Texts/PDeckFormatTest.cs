using System;
using System.IO;
using JUSToolkit.Texts.Converters;
using JUSToolkit.Texts.Formats;
using NUnit.Framework;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUS.Tests.Texts
{
    public class PDeckFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/PDeck/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void PDeckTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (Node node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> PDeck
                    BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2PDeck = new Binary2PDeck();
                    PDeck expectedPDeck = null;
                    try {
                        expectedPDeck = binary2PDeck.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> PDeck with {node.Path}\n{ex}");
                    }

                    // PDeck -> NCF (PDeck)
                    var originalContainer = new NodeContainerFormat();
                    originalContainer.Root.Add(new Node("test", expectedPDeck));

                    // NCF (PDeck) -> Po
                    var pDeck2Po = new PDeck2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = pDeck2Po.Convert(originalContainer);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception NCF (PDeck) -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> NCF (PDeck)
                    NodeContainerFormat container = null;
                    try {
                        container = pDeck2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> PDeck with {node.Path}\n{ex}");
                    }

                    // NCF -> PDeck
                    PDeck actualDeck = container.Root.Children[0].GetFormatAs<PDeck>();

                    // PDeck -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2PDeck.Convert(actualDeck);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception PDeck -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"PDeck are not identical: {node.Path}");
                }
            }
        }
    }
}
