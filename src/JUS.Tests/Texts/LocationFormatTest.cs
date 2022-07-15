using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../" + "Resources/Texts/Location/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void LocationTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.*", SearchOption.AllDirectories)) {
                using (var node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> Location
                    var expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2Location = new Binary2Location();
                    Location expectedStage = null;
                    try {
                        expectedStage = binary2Location.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> Location with {node.Path}\n{ex}");
                    }

                    // Location -> Po
                    var location2Po = new Location2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = location2Po.Convert(expectedStage);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Location -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> Stage
                    Location actualLocation = null;
                    try {
                        actualLocation = location2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> Location with {node.Path}\n{ex}");
                    }

                    // Location -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2Location.Convert(actualLocation);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Location -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"Location is not identical: {node.Path}");
                }
            }
        }
    }
}
