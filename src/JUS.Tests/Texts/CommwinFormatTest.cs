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
    public class CommwinFormatTest
    {
        private string resPath = string.Empty;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/Commwin/");
        }

        [Test]
        public void CommwinTest()
        {
            if (!Directory.Exists(resPath)) {
                Assert.Ignore("The resources folder does not exist");
            }

            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (Node node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> Commwin
                    BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>()!;
                    var binary2Commwin = new Binary2Commwin();
                    Commwin expectedCommwin = null!;
                    try {
                        expectedCommwin = binary2Commwin.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> Commwin with {node.Path}\n{ex}");
                    }

                    // Commwin -> Po
                    var commwin2Po = new Commwin2Po();
                    Po expectedPo = null!;
                    try {
                        expectedPo = commwin2Po.Convert(expectedCommwin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Commwin -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> Commwin
                    Commwin actualCommwin = null!;
                    try {
                        actualCommwin = commwin2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> Commwin with {node.Path}\n{ex}");
                    }

                    // Commwin -> BinaryFormat
                    BinaryFormat actualBin = null!;
                    try {
                        actualBin = binary2Commwin.Convert(actualCommwin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Commwin -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    actualBin.Stream.WriteTo("test.bin");

                    // Comparing Binaries
                    Assert.That(expectedBin.Stream.Compare(actualBin.Stream!), Is.True, $"Commwin are not identical: {node.Path}");
                }
            }
        }
    }
}
