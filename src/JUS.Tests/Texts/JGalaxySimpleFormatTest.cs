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
    public class JGalaxySimpleFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../" + "Resources/Texts/JGalaxy/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void JGalaxySimpleTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (var node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> JGalaxySimple
                    var expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2JGalaxySimple = new Binary2JGalaxySimple();
                    JGalaxySimple expectedJGalaxySimple = null;
                    try {
                        expectedJGalaxySimple = binary2JGalaxySimple.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> JGalaxySimple with {node.Path}\n{ex}");
                    }

                    // JGalaxySimple -> Po
                    var galaxy2Po = new JGalaxySimple2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = galaxy2Po.Convert(expectedJGalaxySimple);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception JGalaxySimple -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> JGalaxySimple
                    JGalaxySimple actualJGalaxySimple = null;
                    try {
                        actualJGalaxySimple = galaxy2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> JGalaxySimple with {node.Path}\n{ex}");
                    }

                    // JGalaxySimple -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2JGalaxySimple.Convert(actualJGalaxySimple);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception JGalaxySimple -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"JGalaxySimple are not identical: {node.Path}");
                }
            }
        }
    }
}
