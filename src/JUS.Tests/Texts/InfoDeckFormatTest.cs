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
    public class InfoDeckFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/InfoDeck/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void InfoDeckTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (Node node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> InfoDeck
                    BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2InfoDeck = new Binary2InfoDeck();
                    InfoDeck expectedInfoDeck = null;
                    try {
                        expectedInfoDeck = binary2InfoDeck.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> InfoDeck with {node.Path}\n{ex}");
                    }

                    // InfoDeck -> Po
                    var infoDeck2Po = new InfoDeck2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = infoDeck2Po.Convert(expectedInfoDeck);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception InfoDeck -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> InfoDeck
                    InfoDeck actualInfoDeck = null;
                    try {
                        actualInfoDeck = infoDeck2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> InfoDeck with {node.Path}\n{ex}");
                    }

                    // InfoDeck -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2InfoDeck.Convert(actualInfoDeck);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception InfoDeck -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"InfoDeck are not identical: {node.Path}");
                }
            }
        }
    }
}
