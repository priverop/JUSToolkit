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
    public class RulemessFormatTest
    {
        private string resPath = string.Empty;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/Rulemess/");

            Assert.That(Directory.Exists(resPath), Is.True, "The resources folder does not exist");
        }

        [Test]
        public void RulemessTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (Node node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> Rulemess
                    BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>()!;
                    var binary2Rulemess = new Binary2Rulemess();
                    Rulemess expectedRulemess = null!;
                    try {
                        expectedRulemess = binary2Rulemess.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> Rulemess with {node.Path}\n{ex}");
                    }

                    // Rulemess -> Po
                    var rulemess2Po = new Rulemess2Po();
                    Po expectedPo = null!;
                    try {
                        expectedPo = rulemess2Po.Convert(expectedRulemess);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Rulemess -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> Rulemess
                    Rulemess actualRulemess = null!;
                    try {
                        actualRulemess = rulemess2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> Rulemess with {node.Path}\n{ex}");
                    }

                    // Rulemess -> BinaryFormat
                    BinaryFormat actualBin = null!;
                    try {
                        actualBin = binary2Rulemess.Convert(actualRulemess);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Rulemess -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.That(expectedBin.Stream.Compare(actualBin.Stream!), Is.True, $"Rulemess are not identical: {node.Path}");
                }
            }
        }
    }
}
