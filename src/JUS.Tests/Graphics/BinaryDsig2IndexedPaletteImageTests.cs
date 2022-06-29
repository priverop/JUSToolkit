// Copyright (c) 2022 Pablo Rivero

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System.Collections;
using System.IO;
using System.Linq;
using JUSToolkit.Graphics;
using NUnit.Framework;
using Texim.Formats;
using Texim.Palettes;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Tests.Graphics
{
    [TestFixture]
    public class BinaryDsig2IndexedPaletteImageTests
    {
        public static IEnumerable GetFiles()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "graphics");
            string listPath = Path.Combine(basePath, "nccg.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]),
                    Path.Combine(basePath, data[2]))
                    .SetName($"({data[0]}, {data[1]}, {data[2]})"));
        }

        [Test]
        public void Guards()
        {
            var converter = new BinaryNccl2PaletteCollection();
            Assert.That(() => converter.Convert(null), Throws.ArgumentNullException);
        }

        [TestCaseSource(nameof(GetFiles))]
        public void DeserializeAndCheckImageHash(string infoPath, string ncclPath, string nccgPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(ncclPath);
            TestDataBase.IgnoreIfFileDoesNotExist(nccgPath);
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);

            var info = BinaryInfo.FromYaml(infoPath);

            using var paletteNode = NodeFactory.FromFile(ncclPath, FileOpenMode.Read)
                .TransformWith<BinaryNccl2PaletteCollection>();

            using var pixelsNode = NodeFactory.FromFile(nccgPath, FileOpenMode.Read)
                .TransformWith<BinaryNccg2IndexedImage>();

            var bitmapParams = new IndexedImageBitmapParams {
                Palettes = paletteNode.GetFormatAs<PaletteCollection>(),
            };
            pixelsNode.TransformWith<IndexedImage2Bitmap, IndexedImageBitmapParams>(bitmapParams)
                .Stream.Should().MatchInfo(info);
        }
    }
}