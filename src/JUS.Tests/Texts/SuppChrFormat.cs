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
    public class SuppChrFormat
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../" + "Resources/Texts/SuppChr/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void SuppChrTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.*", SearchOption.AllDirectories)) {
                using (var node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> SuppChr
                    var expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2SuppChr = new Binary2SuppChr();
                    SuppChr expectedSuppChr = null;
                    try {
                        expectedSuppChr = binary2SuppChr.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> SuppChr with {node.Path}\n{ex}");
                    }

                    // SuppChr -> Po
                    var suppChr2Po = new SuppChr2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = suppChr2Po.Convert(expectedSuppChr);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception SuppChr -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> SuppChr
                    SuppChr actualSuppChr = null;
                    try {
                        actualSuppChr = suppChr2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> SuppChr with {node.Path}\n{ex}");
                    }

                    // SuppChr -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2SuppChr.Convert(actualSuppChr);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception SuppChr -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"SuppChr is not identical: {node.Path}");
                }
            }
        }
    }
}
