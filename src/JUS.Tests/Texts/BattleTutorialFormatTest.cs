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
    public class BattleTutorialFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../" + "Resources/Texts/BattleTutorial/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void BattleTutorialTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (var node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> BattleTutorial
                    var expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2BattleTutorial = new Binary2BattleTutorial();
                    BattleTutorial expectedBattleTutorial = null;
                    try {
                        expectedBattleTutorial = binary2BattleTutorial.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> BattleTutorial with {node.Path}\n{ex}");
                    }

                    // BattleTutorial -> Po
                    var battleTutorial2Po = new BattleTutorial2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = battleTutorial2Po.Convert(expectedBattleTutorial);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BattleTutorial -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> BattleTutorial
                    BattleTutorial actualBattleTutorial = null;
                    try {
                        actualBattleTutorial = battleTutorial2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> BattleTutorial with {node.Path}\n{ex}");
                    }

                    // BattleTutorial -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2BattleTutorial.Convert(actualBattleTutorial);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BattleTutorial -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"BattleTutorial are not identical: {node.Path}");
                }
            }
        }
    }
}
