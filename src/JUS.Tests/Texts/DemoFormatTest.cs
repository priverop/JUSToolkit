using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUSToolkit.Texts.Converters;
using JUSToolkit.Texts.Formats;
using NUnit.Framework;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUS.Tests.Texts
{
    public class DemoFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../" + "Resources/Texts/Demo/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void DemoTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.*", SearchOption.AllDirectories)) {
                using (var node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> Demo
                    var expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2Demo = new Binary2Demo();
                    Demo expectedDemo = null;
                    try {
                        expectedDemo = binary2Demo.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> Demo with {node.Path}\n{ex}");
                    }

                    // Demo -> Po
                    var demo2Po = new Demo2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = demo2Po.Convert(expectedDemo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Demo -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> Demo
                    Demo actualDemo = null;
                    try {
                        actualDemo = demo2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> Demo with {node.Path}\n{ex}");
                    }

                    // Demo -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2Demo.Convert(actualDemo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Demo -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"Demo are not identical: {node.Path}");
                }
            }
        }
    }
}
