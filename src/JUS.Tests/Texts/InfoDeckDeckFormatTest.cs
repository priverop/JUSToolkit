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
    public class InfoDeckDeckFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/InfoDeckDeck/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void InfoDeckDeckTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (Node node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> InfoDeckDeck
                    BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2InfoDeckDeck = new Binary2InfoDeckDeck();
                    InfoDeckDeck expectedInfoDeckDeck = null;
                    try {
                        expectedInfoDeckDeck = binary2InfoDeckDeck.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> InfoDeckDeck with {node.Path}\n{ex}");
                    }

                    // InfoDeckDeck -> Po
                    var infoDeckDeck2Po = new InfoDeckDeck2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = infoDeckDeck2Po.Convert(expectedInfoDeckDeck);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception InfoDeckDeck -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> InfoDeckDeck
                    InfoDeckDeck actualInfoDeckDeck = null;
                    try {
                        actualInfoDeckDeck = infoDeckDeck2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> InfoDeckDeck with {node.Path}\n{ex}");
                    }

                    // InfoDeckDeck -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2InfoDeckDeck.Convert(actualInfoDeckDeck);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception InfoDeckDeck -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"InfoDeckDeck are not identical: {node.Path}");
                }
            }
        }
    }
}
