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
    public class PieceFormatTest
    {
        private string resPath = string.Empty;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../Resources/Texts/Piece/piece.bin");
        }

        [Test]
        public void PieceTest()
        {
            if (!Directory.Exists(resPath)) {
                Assert.Ignore("The resources folder does not exist");
            }

            Node node = NodeFactory.FromFile(resPath);

            // BinaryFormat -> Piece
            BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>()!;
            var binary2Piece = new Binary2Piece();
            Piece expectedPiece = null!;
            try {
                expectedPiece = binary2Piece.Convert(expectedBin);
            } catch (Exception ex) {
                Assert.Fail($"Exception BinaryFormat -> Piece with {node.Path}\n{ex}");
            }

            // Piece -> Po
            var piece2Po = new Piece2Po();
            Po expectedPo = null!;
            try {
                expectedPo = piece2Po.Convert(expectedPiece);
            } catch (Exception ex) {
                Assert.Fail($"Exception Piece -> Po with {node.Path}\n{ex}");
            }

            // Po -> Piece
            Piece actualPiece = null!;
            try {
                actualPiece = piece2Po.Convert(expectedPo);
            } catch (Exception ex) {
                Assert.Fail($"Exception Po -> Piece with {node.Path}\n{ex}");
            }

            // Piece -> BinaryFormat
            BinaryFormat actualBin = null!;
            try {
                actualBin = binary2Piece.Convert(actualPiece);
            } catch (Exception ex) {
                Assert.Fail($"Exception Piece -> BinaryFormat with {node.Path}\n{ex}");
            }

            // Comparing Binaries
            Assert.That(expectedBin.Stream.Compare(actualBin.Stream!), Is.True, $"Piece is not identical: {node.Path}");
        }
    }
}
