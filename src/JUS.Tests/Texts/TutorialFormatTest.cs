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
    public class TutorialFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/Tutorial/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void TutorialTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (Node node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> Tutorial
                    BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2Tutorial = new Binary2Tutorial();
                    Tutorial expectedTutorial = null;
                    try {
                        expectedTutorial = binary2Tutorial.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> Tutorial with {node.Path}\n{ex}");
                    }

                    // Tutorial -> Po
                    var tutorial2Po = new Tutorial2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = tutorial2Po.Convert(expectedTutorial);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Tutorial -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> Tutorial
                    Tutorial actualTutorial = null;
                    try {
                        actualTutorial = tutorial2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> Tutorial with {node.Path}\n{ex}");
                    }

                    // Tutorial -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2Tutorial.Convert(actualTutorial);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Tutorial -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"Tutorial are not identical: {node.Path}");
                }
            }
        }
    }
}
