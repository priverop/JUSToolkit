// Copyright(c) 2022 Priverop
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using FluentAssertions;
using JUS.Tool.Containers;
using JUS.Tool.Containers.Converters;
using JUS.Tool.Graphics;
using JUS.Tool.Graphics.Converters;
using JUS.Tool.Utils;
using NUnit.Framework;
using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tests.Graphics
{
    [TestFixture]
    public class DigTests
    {
        private static readonly Lazy<NitroRom> Root = new(TestDataBase.ReadSoftware);
        private static Alar? TopMenuAlar;

        public static IEnumerable<TestCaseData> GetDigPaths()
        {
            if (!File.Exists(TestDataBase.SoftwareNitroRomPath)) {
                return [];
            }

            Node topmenuAlar = Navigator.SearchNode(Root.Value.Data, "topmenu/topmenu.aar") ?? throw new ArgumentException("topmenu.aar not found");
            TopMenuAlar = topmenuAlar.GetFormatAs<Alar>() ?? topmenuAlar.TransformWith<Binary2Alar>().GetFormatAs<Alar>()!;

            return Navigator.IterateNodes(TopMenuAlar.Root, NavigationMode.DepthFirst)
                .Where(n => n.Name == "top_bg01.dig")
                .Select(n => new TestCaseData(n.Path));
        }

        [TestCaseSource(nameof(GetDigPaths))]
        public void TwoWaysIdenticalDigImage(string digPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(TestDataBase.SoftwareNitroRomPath);

            Node originalDig = Navigator.SearchNode(TopMenuAlar!.Root, digPath) ?? throw new ArgumentException($"{digPath} not found");
            Assert.That(originalDig, Is.Not.Null);

            string atmPath = Path.ChangeExtension(digPath, ".atm");

            Node originalAtm = Navigator.SearchNode(TopMenuAlar!.Root, atmPath) ?? throw new ArgumentException($"{atmPath} not found");
            Assert.That(originalAtm, Is.Not.Null);

            AssertTwoWaysIdenticalDigImage(originalDig, originalAtm);
        }

        private static void AssertTwoWaysIdenticalDigImage(Node dig, Node atm)
        {
            // Clone original nodes
            var digCloneBinary = (BinaryFormat)new BinaryFormat(dig.Stream!).DeepClone();
            var atmCloneBinary = (BinaryFormat)new BinaryFormat(atm.Stream!).DeepClone();
            using var digClone = new Node(dig.Name, digCloneBinary);
            using var atmClone = new Node(atm.Name, atmCloneBinary);

            // Export Dig + ATM
            var binaryDig2Bitmap = new BinaryDig2Bitmap(atmClone);
            BinaryFormat originalPng = binaryDig2Bitmap.Convert(digClone.GetFormatAs<IBinary>()!);
            var originalPngClone = (BinaryFormat)new BinaryFormat(originalPng.Stream!).DeepClone();

            // Import png
            var digImportBinary = (BinaryFormat)new BinaryFormat(dig.Stream!).DeepClone();
            var atmImportBinary = (BinaryFormat)new BinaryFormat(atm.Stream!).DeepClone();
            using var digImport = new Node(dig.Name, digImportBinary);
            using var atmImport = new Node(atm.Name, atmImportBinary);
            var converter = new Png2DigAtm(digImport, atmImport, true);
            NodeContainerFormat transformedFiles = converter.Convert(new Node("png", originalPng));

            // Export new Dig + ATM
            Node newDig = transformedFiles.Root.Children[dig.Name]!;
            Node newAtm = transformedFiles.Root.Children[atm.Name]!;

            var converter2 = new BinaryDig2Bitmap(newAtm);
            BinaryFormat finalPng = converter2.Convert(newDig.GetFormatAs<IBinary>()!);

            // Are the PNGs equal?
            bool hasSameLength = originalPng.Stream.Length == finalPng.Stream.Length;
            if (!hasSameLength) {
                string testCaseName = Path.GetFileNameWithoutExtension(digClone.Name);
                TestDataBase.WriteFailedData(originalPngClone.Stream, $"expected_{testCaseName}.png");
                TestDataBase.WriteFailedData(finalPng.Stream, $"actual_{testCaseName}.png");
            }

            finalPng.Stream.Length.Should().Be(originalPngClone.Stream.Length);
            finalPng.Stream.Compare(originalPngClone.Stream).Should().BeTrue();
        }

        [TestCaseSource(nameof(GetDigPaths))]
        public void ImportEmptyPalettes(string digPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(TestDataBase.SoftwareNitroRomPath);

            Node originalDig = Navigator.SearchNode(TopMenuAlar!.Root, digPath) ?? throw new ArgumentException($"{digPath} not found");
            Assert.That(originalDig, Is.Not.Null);

            string atmPath = Path.ChangeExtension(digPath, ".atm");

            Node originalAtm = Navigator.SearchNode(TopMenuAlar!.Root, atmPath) ?? throw new ArgumentException($"{atmPath} not found");
            Assert.That(originalAtm, Is.Not.Null);

            AssertImportEmptyPalettes(originalDig, originalAtm);
        }

        private static void AssertImportEmptyPalettes(Node dig, Node atm)
        {
            // Clone original nodes
            var digCloneBinary = (BinaryFormat)new BinaryFormat(dig.Stream!).DeepClone();
            var atmCloneBinary = (BinaryFormat)new BinaryFormat(atm.Stream!).DeepClone();
            using var digClone = new Node(dig.Name, digCloneBinary);
            using var atmClone = new Node(atm.Name, atmCloneBinary);

            // Export Dig + ATM into a PNG
            var binaryDig2Bitmap = new BinaryDig2Bitmap(atmClone);
            BinaryFormat originalPng = binaryDig2Bitmap.Convert(digClone.GetFormatAs<IBinary>()!);

            // Import PNG
            var digImportBinary = (BinaryFormat)new BinaryFormat(dig.Stream!).DeepClone();
            var atmImportBinary = (BinaryFormat)new BinaryFormat(atm.Stream!).DeepClone();
            using var digImport = new Node(dig.Name, digImportBinary);
            using var atmImport = new Node(atm.Name, atmImportBinary);
            var converter = new Png2DigAtm(digImport, atmImport, true);
            NodeContainerFormat transformedFiles = converter.Convert(new Node("png", originalPng));

            Node newDigNode = transformedFiles.Root.Children[dig.Name]!;
            Node newAtmNode = transformedFiles.Root.Children[atm.Name]!;

            Dig newDig = newDigNode
                .TransformWith<LzssDecompression>()
                .TransformWith<Binary2Dig>()
                .GetFormatAs<Dig>()!;
            Almt newAtm = newAtmNode
                .TransformWith<LzssDecompression>()
                .TransformWith<Binary2Almt>()
                .GetFormatAs<Almt>()!;

            // The DIG files have a lot of empty palettes (all colors are 0,0,0: black).
            // In order to import PNGs with black pixels, Texim looks for that black color, and finds
            // the first palette (which is empty, all black).
            // We fixed this, so we shouldn't have empty palettes linked in the map.

            // First we find the palettes the map is using:
            IEnumerable<byte> usedPalettes = newAtm.Maps.Select(m => m.PaletteIndex).Distinct();
            // Let's now iterate these palettes, and check for empty-palettes (all black):
            IEnumerable<byte> blackPalettes = usedPalettes.Where(index =>
                newDig.Palettes[index].Colors.All(color => color.Red == 0 && color.Green == 0 && color.Blue == 0
                )
            );

            blackPalettes.Should().BeEmpty();
        }

        public static IEnumerable<TestCaseData> GetFiles()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Graphics");
            string listPath = Path.Combine(basePath, "dig.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]),
                    Path.Combine(basePath, data[2]))
                    .SetName($"({data[0]}, {data[1]}, {data[2]})"));
        }

        [TestCaseSource(nameof(GetFiles))]
        public void DeserializeAndCheckFileHash(string infoPath, string digPath, string atmPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(digPath);
            TestDataBase.IgnoreIfFileDoesNotExist(atmPath);

            var info = BinaryInfo.FromYaml(infoPath);

            using Node mapsNode = NodeFactory.FromFile(atmPath, FileOpenMode.Read);

            using Node pixelsPaletteNode = NodeFactory.FromFile(digPath, FileOpenMode.Read)
                .TransformWith(new BinaryDig2Bitmap(mapsNode));

            pixelsPaletteNode.Stream.Should().MatchInfo(info);
        }

        [TestCaseSource(nameof(GetFiles))]
        public void TwoWaysIdenticalDigStream(string infoPath, string digPath, string atmPath)
        {
            // TODO: refactor first the Dig import logic into a converter
            Assert.Ignore("Imported Dig are smaller, we neet to test with PNGs instead");
            TestDataBase.IgnoreIfFileDoesNotExist(digPath);

            using Node node = NodeFactory.FromFile(digPath, FileOpenMode.Read);

            Dig dig = node.GetFormatAs<IBinary>().ConvertWith(new Binary2Dig());
            BinaryFormat generatedStream = dig.ConvertWith(new Dig2Binary());

            var originalStream = new DataStream(node.Stream, 0, node.Stream.Length);
            generatedStream.Stream.Length.Should().Be(originalStream.Length);
            generatedStream.Stream.Compare(originalStream).Should().BeTrue();
        }
    }
}
