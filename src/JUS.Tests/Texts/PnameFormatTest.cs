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
    public class PnameFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/Pname/pname.bin");

            Assert.True(File.Exists(resPath), "The resource file does not exist", resPath);
        }

        [Test]
        public void PnameTest()
        {
            Node node = NodeFactory.FromFile(resPath);

            // BinaryFormat -> Pname
            BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>();
            var binary2Pname = new Binary2Pname();
            Pname expectedPname = null;
            try {
                expectedPname = binary2Pname.Convert(expectedBin);
            } catch (Exception ex) {
                Assert.Fail($"Exception BinaryFormat -> Pname with {node.Path}\n{ex}");
            }

            // Pname -> Po
            var pname2Po = new Pname2Po();
            Po expectedPo = null;
            try {
                expectedPo = pname2Po.Convert(expectedPname);
            } catch (Exception ex) {
                Assert.Fail($"Exception Pname -> Po with {node.Path}\n{ex}");
            }

            // Po -> Pname
            Pname actualPname = null;
            try {
                actualPname = pname2Po.Convert(expectedPo);
            } catch (Exception ex) {
                Assert.Fail($"Exception Po -> Pname with {node.Path}\n{ex}");
            }

            // Pname -> BinaryFormat
            BinaryFormat actualBin = null;
            try {
                actualBin = binary2Pname.Convert(actualPname);
            } catch (Exception ex) {
                Assert.Fail($"Exception Pname -> BinaryFormat with {node.Path}\n{ex}");
            }

            // Comparing Binaries
            Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"Pname is not identical: {node.Path}");
        }
    }
}
