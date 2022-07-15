using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUS.Tool.Texts.Converters;
using JUS.Tool.Texts.Formats;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using NUnit.Framework;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUS.Tests.Texts
{
    public class BgmFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../" + "Resources/Texts/Bgm/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void BgmTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.*", SearchOption.AllDirectories)) {
                using (var node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> Bgm
                    var expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2Bgm = new Binary2Bgm();
                    Bgm expectedBgm = null;
                    try {
                        expectedBgm = binary2Bgm.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> Bgm with {node.Path}\n{ex}");
                    }

                    // Bgm -> Po
                    var bgm2Po = new Bgm2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = bgm2Po.Convert(expectedBgm);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Bgm -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> Bgm
                    Bgm actualBgm = null;
                    try {
                        actualBgm = bgm2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> Bgm with {node.Path}\n{ex}");
                    }

                    // Bgm -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2Bgm.Convert(actualBgm);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Bgm -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"Bgm are not identical: {node.Path}");
                }
            }
        }
    }
}
