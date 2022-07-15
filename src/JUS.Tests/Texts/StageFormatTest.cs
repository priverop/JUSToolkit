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
    public class StageFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../" + "Resources/Texts/Stage/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void StageTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.*", SearchOption.AllDirectories)) {
                using (var node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> Stage
                    var expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2Stage = new Binary2Stage();
                    Stage expectedStage = null;
                    try {
                        expectedStage = binary2Stage.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> Stage with {node.Path}\n{ex}");
                    }

                    // Stage -> Po
                    var stage2Po = new Stage2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = stage2Po.Convert(expectedStage);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Stage -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> Stage
                    Stage actualStage = null;
                    try {
                        actualStage = stage2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> Stage with {node.Path}\n{ex}");
                    }

                    // Stage -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2Stage.Convert(actualStage);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Stage -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"Stage are not identical: {node.Path}");
                }
            }
        }
    }
}
