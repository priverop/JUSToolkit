using System;
using System.IO;
using JUS.Tool.Texts.Converters;
using JUS.Tool.Texts.Formats;
using NUnit.Framework;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUS.Tests.Texts
{
    public class SimpleBinFormatTest
    {
        private string resPath = string.Empty;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/SimpleBin/");

            Assert.That(Directory.Exists(resPath), Is.True, "The resources folder does not exist");
        }

        [Test]
        public void SimpleBinTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (Node node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> SimpleBin
                    BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>()!;
                    var binary2SimpleBin = new Binary2SimpleBin();
                    SimpleBin expectedSimpleBin = null!;
                    try {
                        expectedSimpleBin = binary2SimpleBin.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> SimpleBin with {node.Path}\n{ex}");
                    }

                    // SimpleBin -> Po
                    var simpleBin2Po = new SimpleBin2Po();
                    Po expectedPo = null!;
                    try {
                        expectedPo = simpleBin2Po.Convert(expectedSimpleBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception SimpleBin -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> SimpleBin
                    SimpleBin actualSimpleBin = null!;
                    try {
                        actualSimpleBin = simpleBin2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> SimpleBin with {node.Path}\n{ex}");
                    }

                    // SimpleBin -> BinaryFormat
                    BinaryFormat actualBin = null!;
                    try {
                        actualBin = binary2SimpleBin.Convert(actualSimpleBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception SimpleBin -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.That(expectedBin.Stream.Compare(actualBin.Stream!), Is.True, $"SimpleBin are not identical: {node.Path}");
                }
            }
        }
    }
}
