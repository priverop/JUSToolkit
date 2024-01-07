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
    public class KomatxtFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/Komatxt/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void KomatxtTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (Node node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> Komatxt
                    BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2Komatxt = new Binary2Komatxt();
                    Komatxt expectedKomatxt = null;
                    try {
                        expectedKomatxt = binary2Komatxt.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> Komatxt with {node.Path}\n{ex}");
                    }

                    // Komatxt -> Po
                    var komatxt2Po = new Komatxt2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = komatxt2Po.Convert(expectedKomatxt);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Komatxt -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> Komatxt
                    Komatxt actualKomatxt = null;
                    try {
                        actualKomatxt = komatxt2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> Komatxt with {node.Path}\n{ex}");
                    }

                    // Komatxt -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2Komatxt.Convert(actualKomatxt);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Komatxt -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"Komatxt are not identical: {node.Path}");
                }
            }
        }
    }
}
