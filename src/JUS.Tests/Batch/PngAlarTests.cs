// Copyright(c) 2024 Priverop
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using JUSToolkit.BatchConverters;
using JUSToolkit.Containers;
using JUSToolkit.Containers.Converters;
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using JUSToolkit.Utils;
using NUnit.Framework;
using Texim.Compressions.Nitro;
using Texim.Formats;
using Texim.Images;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Tests.Batch
{
    [TestFixture]
    public class PngAlarTests
    {
        public static IEnumerable<TestCaseData> GetFiles()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Batch");
            string listPath = Path.Combine(basePath, "import.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]))
                    .SetName($"({data[0]}, {data[1]}"));
        }

        // Pleonex's NDS Image Compression algorithm is better than Nintendo's, so the output .aar is smaller than the original.
        // So we test that the original png and the output one is the same:
        // PNG + Alar -> Alar -> PNG
        [TestCaseSource(nameof(GetFiles))]
        public void TwoWaysIdenticalPngStream(string alarPath, string pngPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(alarPath);
            TestDataBase.IgnoreIfFileDoesNotExist(pngPath);

            using Node originalAlar = NodeFactory.FromFile(alarPath, FileOpenMode.Read)
            .TransformWith<Binary2Alar3>();
            using Node inputPNG = NodeFactory.FromFile(pngPath, FileOpenMode.Read);

            var originalStream = new DataStream(inputPNG.Stream!, 0, inputPNG.Stream.Length);

            string originalName = Path.GetFileNameWithoutExtension(pngPath);

            var png2Alar3 = new Png2Alar3(inputPNG, originalName + ".dig", originalName + ".atm");

            Alar3 newAlar = originalAlar
                .TransformWith(png2Alar3)
                .GetFormatAs<Alar3>();

            // Extracting the png from the newAlar to compare it with the original
            Node newDig = Navigator.IterateNodes(newAlar.Root).First(n => n.Name == originalName + ".dig") ?? throw new FormatException("Dig doesn't exist: " + originalName + ".dig");
            Node newAtm = Navigator.IterateNodes(newAlar.Root).First(n => n.Name == originalName + ".atm") ?? throw new FormatException("Atm doesn't exist: " + originalName + ".atm");

            var binaryDig2Bitmap = new BinaryDig2Bitmap(newAtm);

            using Node pixelsPaletteNode = newDig.TransformWith(binaryDig2Bitmap);

            pixelsPaletteNode.Stream.WriteTo("testing_.png");

            pixelsPaletteNode.Stream.Length.Should().Be(originalStream.Length);
            pixelsPaletteNode.Stream.Compare(originalStream).Should().BeTrue();
        }
    }
}
