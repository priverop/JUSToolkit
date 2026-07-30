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
    public class JGalaxySimpleFormatTest
    {
        private string resPath = string.Empty;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/JGalaxySimple/");

            Assert.That(Directory.Exists(resPath), Is.True, "The resources folder does not exist");
        }

        [Test]
        public void JGalaxySimpleTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (Node node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> JGalaxySimple
                    BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>()!;
                    var binary2JGalaxySimple = new Binary2JGalaxySimple();
                    JGalaxySimple expectedJGalaxySimple = null!;
                    try {
                        expectedJGalaxySimple = binary2JGalaxySimple.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> JGalaxySimple with {node.Path}\n{ex}");
                    }

                    // JGalaxySimple -> Po
                    var galaxy2Po = new JGalaxySimple2Po();
                    Po expectedPo = null!;
                    try {
                        expectedPo = galaxy2Po.Convert(expectedJGalaxySimple);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception JGalaxySimple -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> JGalaxySimple
                    JGalaxySimple actualJGalaxySimple = null!;
                    try {
                        actualJGalaxySimple = galaxy2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> JGalaxySimple with {node.Path}\n{ex}");
                    }

                    // JGalaxySimple -> BinaryFormat
                    BinaryFormat actualBin = null!;
                    try {
                        actualBin = binary2JGalaxySimple.Convert(actualJGalaxySimple);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception JGalaxySimple -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.That(expectedBin.Stream.Compare(actualBin.Stream!), Is.True, $"JGalaxySimple are not identical: {node.Path}");
                }
            }
        }
    }
}
