// Copyright(c) 2022 Pablo Rivero
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
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using NUnit.Framework;
using Texim.Compressions.Nitro;
using Texim.Formats;
using Texim.Images;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Tests.Graphics
{
    [TestFixture]
    public class DigTests
    {
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

            // Pixels + Palette
            using var pixelsPaletteNode = NodeFactory.FromFile(digPath, FileOpenMode.Read)
                .TransformWith<BinaryDsig2IndexedPaletteImage>();

            // Map
            using var mapsNode = NodeFactory.FromFile(atmPath, FileOpenMode.Read)
                .TransformWith<Binary2Almt>();

            var mapsParams = new MapDecompressionParams {
                Map = mapsNode.GetFormatAs<Almt>(),
                TileSize = mapsNode.GetFormatAs<Almt>().TileSize,
            };
            var bitmapParams = new IndexedImageBitmapParams {
                Palettes = pixelsPaletteNode.GetFormatAs<IndexedPaletteImage>(),
            };

            pixelsPaletteNode.TransformWith<MapDecompression, MapDecompressionParams>(mapsParams)
                .TransformWith<IndexedImage2Bitmap, IndexedImageBitmapParams>(bitmapParams)
                .Stream.Should().MatchInfo(info);
        }

        [TestCaseSource(nameof(GetFiles))]
        public void TwoWaysIdenticalDigStream(string infoPath, string digPath, string atmPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(digPath);

            using Node node = NodeFactory.FromFile(digPath, FileOpenMode.Read);

            var dig = (Dig)ConvertFormat.With<Binary2Dig>(node.Format!);
            var generatedStream = (BinaryFormat)ConvertFormat.With<Dig2Binary>(dig);

            var originalStream = new DataStream(node.Stream!, 0, node.Stream.Length);
            generatedStream.Stream.Length.Should().Be(originalStream.Length);
            generatedStream.Stream.Compare(originalStream).Should().BeTrue();
        }
    }
}
