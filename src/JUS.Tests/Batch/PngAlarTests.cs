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

        [TestCaseSource(nameof(GetFiles))]
        public void TwoWaysIdenticalAlarStream(string alarPath, string pngPath)
        {
            Assert.Ignore();
            TestDataBase.IgnoreIfFileDoesNotExist(alarPath);
            TestDataBase.IgnoreIfFileDoesNotExist(pngPath);

            using Node originalAlar = NodeFactory.FromFile(alarPath, FileOpenMode.Read);
            using Node nodePng = NodeFactory.FromFile(pngPath, FileOpenMode.Read);

            NodeContainerFormat inputPngs = new NodeContainerFormat();
            // Continuar

            var png2Alar3 = new Png2Alar3(originalAlar);

            Alar3 alar = nodePng
                .TransformWith(png2Alar3)
                .GetFormatAs<Alar3>();

            using BinaryFormat generatedStream = alar.ConvertWith(new Alar3ToBinary());

            var originalStream = new DataStream(originalAlar.Stream!, 0, originalAlar.Stream.Length);
            generatedStream.Stream.Length.Should().Be(originalStream.Length);
            generatedStream.Stream.Compare(originalStream).Should().BeTrue();
        }
    }
}
