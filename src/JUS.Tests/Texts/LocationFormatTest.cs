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
    public class LocationFormatTest
    {
        private string resPath = string.Empty;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/Location/");

            Assert.That(Directory.Exists(resPath), Is.True, "The resources folder does not exist");
        }

        [Test]
        public void LocationTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (Node node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> Location
                    BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>()!;
                    var binary2Location = new Binary2Location();
                    Location expectedStage = null!;
                    try {
                        expectedStage = binary2Location.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> Location with {node.Path}\n{ex}");
                    }

                    // Location -> Po
                    var location2Po = new Location2Po();
                    Po expectedPo = null!;
                    try {
                        expectedPo = location2Po.Convert(expectedStage);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Location -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> Stage
                    Location actualLocation = null!;
                    try {
                        actualLocation = location2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> Location with {node.Path}\n{ex}");
                    }

                    // Location -> BinaryFormat
                    BinaryFormat actualBin = null!;
                    try {
                        actualBin = binary2Location.Convert(actualLocation);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Location -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.That(expectedBin.Stream.Compare(actualBin.Stream!), Is.True, $"Location is not identical: {node.Path}");
                }
            }
        }
    }
}
