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
    public class InfoDeckInfoFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/InfoDeckInfo/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void InfoDeckInfoTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (Node node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> InfoDeckInfo
                    BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2InfoDeckInfo = new Binary2InfoDeckInfo();
                    InfoDeckInfo expectedInfoDeckInfo = null;
                    try {
                        expectedInfoDeckInfo = binary2InfoDeckInfo.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> InfoDeckInfo with {node.Path}\n{ex}");
                    }

                    // InfoDeckInfo -> Po
                    var infoDeckInfo2Po = new InfoDeckInfo2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = infoDeckInfo2Po.Convert(expectedInfoDeckInfo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception InfoDeckInfo -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> InfoDeckInfo
                    InfoDeckInfo actualInfoDeckInfo = null;
                    try {
                        actualInfoDeckInfo = infoDeckInfo2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> InfoDeckInfo with {node.Path}\n{ex}");
                    }

                    // InfoDeckInfo -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2InfoDeckInfo.Convert(actualInfoDeckInfo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception InfoDeckInfo -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"InfoDeckInfo are not identical: {node.Path}");
                }
            }
        }
    }
}
