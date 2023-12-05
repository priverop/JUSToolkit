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
    public class JGalaxyComplexFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/JGalaxyComplex/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void JGalaxyComplexTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (Node node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> JGalaxyComplex
                    BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2JGalaxyComplex = new Binary2JGalaxyComplex();
                    JGalaxyComplex expectedJGalaxyComplex = null;
                    try {
                        expectedJGalaxyComplex = binary2JGalaxyComplex.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> JGalaxyComplex with {node.Path}\n{ex}");
                    }

                    // JGalaxyComplex -> Po
                    var galaxy2Po = new JGalaxyComplex2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = galaxy2Po.Convert(expectedJGalaxyComplex);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception JGalaxyComplex -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> JGalaxyComplex
                    JGalaxyComplex actualJGalaxyComplex = null;
                    try {
                        actualJGalaxyComplex = galaxy2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> JGalaxyComplex with {node.Path}\n{ex}");
                    }

                    // JGalaxyComplex -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2JGalaxyComplex.Convert(actualJGalaxyComplex);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception JGalaxyComplex -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"JGalaxyComplex are not identical: {node.Path}");
                }
            }
        }
    }
}
