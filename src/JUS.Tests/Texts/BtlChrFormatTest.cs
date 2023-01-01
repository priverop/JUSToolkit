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
    public class BtlChrFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../" + "Resources/Texts/BtlChr/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void BtlChrTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (var node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> BtlChr
                    var expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2BtlChr = new Binary2BtlChr();
                    BtlChr expectedBtlChr = null;
                    try {
                        expectedBtlChr = binary2BtlChr.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> BtlChr with {node.Path}\n{ex}");
                    }

                    // BtlChr -> Po
                    var btlChr2Po = new BtlChr2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = btlChr2Po.Convert(expectedBtlChr);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BtlChr -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> BtlChr
                    BtlChr actualBtlChr = null;
                    try {
                        actualBtlChr = btlChr2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> BtlChr with {node.Path}\n{ex}");
                    }

                    // BtlChr -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2BtlChr.Convert(actualBtlChr);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Stage -> BtlChr with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"BtlChr is not identical: {node.Path}");
                }
            }
        }
    }
}
