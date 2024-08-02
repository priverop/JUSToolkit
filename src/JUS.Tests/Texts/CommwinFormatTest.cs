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
    public class CommwinFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/Commwin/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void CommwinTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (Node node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> Commwin
                    BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2Commwin = new Binary2Commwin();
                    Commwin expectedCommwin = null;
                    try {
                        expectedCommwin = binary2Commwin.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> Commwin with {node.Path}\n{ex}");
                    }

                    // Commwin -> Po
                    var commwin2Po = new Commwin2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = commwin2Po.Convert(expectedCommwin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Commwin -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> Commwin
                    Commwin actualCommwin = null;
                    try {
                        actualCommwin = commwin2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> Commwin with {node.Path}\n{ex}");
                    }

                    // Commwin -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2Commwin.Convert(actualCommwin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Commwin -> BinaryFormat with {node.Path}\n{ex}");
                    }
                    actualBin.Stream.WriteTo("test.bin");
                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"Commwin are not identical: {node.Path}");
                }
            }
        }
    }
}
