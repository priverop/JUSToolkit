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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using NUnit.Framework;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Tests.Graphics
{
    [TestFixture]
    public class AlmtTests
    {
        public static IEnumerable<TestCaseData> GetFiles()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Graphics");
            string listPath = Path.Combine(basePath, "almt.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]))
                    .SetName($"({data[0]})"));
        }

        [TestCaseSource(nameof(GetFiles))]
        public void TwoWaysIdenticalAlmtStream(string almtPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(almtPath);

            using Node node = NodeFactory.FromFile(almtPath, FileOpenMode.Read);

            Almt almt = new Binary2Almt().Convert(node.GetFormatAs<BinaryFormat>());
            BinaryFormat generatedStream = new Almt2Binary().Convert(almt);

            var originalStream = new DataStream(node.Stream!, 0, node.Stream.Length);
            generatedStream.Stream.Length.Should().Be(originalStream.Length);
            generatedStream.Stream.Compare(originalStream).Should().BeTrue();
        }
    }
}
