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
    public class SimpleBinFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../" + "Resources/Texts/SimpleBin/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void SimpleBinTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (var node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> SimpleBin
                    var expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2SimpleBin = new Binary2SimpleBin();
                    SimpleBin expectedSimpleBin = null;
                    try {
                        expectedSimpleBin = binary2SimpleBin.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> SimpleBin with {node.Path}\n{ex}");
                    }

                    // SimpleBin -> Po
                    var simpleBin2Po = new SimpleBin2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = simpleBin2Po.Convert(expectedSimpleBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception SimpleBin -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> SimpleBin
                    SimpleBin actualSimpleBin = null;
                    try {
                        actualSimpleBin = simpleBin2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> SimpleBin with {node.Path}\n{ex}");
                    }

                    // SimpleBin -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2SimpleBin.Convert(actualSimpleBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception SimpleBin -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"SimpleBin are not identical: {node.Path}");
                }
            }
        }
    }
}
