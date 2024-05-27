// Copyright(c) 2023 Priverop
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
using JUSToolkit.Texts.Converters;
using JUSToolkit.Texts.Formats;
using NUnit.Framework;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUSToolkit.Tests.Containers
{
    [TestFixture]
    public class JQuizFormatTest
    {
        public static IEnumerable<TestCaseData> GetJQuizFiles()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Texts/JQuiz");
            string listPath = Path.Combine(basePath, "jquiz.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]))
                    .SetName($"({data[0]}, {data[1]})"));
        }

        // We can't use the yml to check the deserialized stream because our JQuiz is not a NodeContainerFormat.
        [TestCaseSource(nameof(GetJQuizFiles))]
        public void DeserializeJQuiz(string infoPath, string jquizPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(jquizPath);
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);

            using Node jquiz = NodeFactory.FromFile(jquizPath, FileOpenMode.Read);

            jquiz.Invoking(n => n.TransformWith<Binary2JQuiz>()).Should().NotThrow();
            jquiz.GetFormatAs<JQuiz>().Entries.Should().NotBeEmpty().And.HaveCount(3006);
        }

        [TestCaseSource(nameof(GetJQuizFiles))]
        public void TwoWaysIdenticalJQuizStream(string infoPath, string jquizPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(jquizPath);

            using Node node = NodeFactory.FromFile(jquizPath, FileOpenMode.Read);

            JQuiz jquiz = new Binary2JQuiz().Convert(node.GetFormatAs<BinaryFormat>());
            BinaryFormat generatedStream = new Binary2JQuiz().Convert(jquiz);

            generatedStream.Stream.Length.Should().Be(node.Stream!.Length);
            generatedStream.Stream.Compare(node.Stream).Should().BeTrue();
        }

        [TestCaseSource(nameof(GetJQuizFiles))]
        public void JQuizTest(string infoPath, string jquizPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(jquizPath);

            using Node node = NodeFactory.FromFile(jquizPath);

            // BinaryFormat -> JQuiz
            BinaryFormat expectedBin = node.GetFormatAs<BinaryFormat>();
            var binary2JQuiz = new Binary2JQuiz();
            JQuiz expectedJQuiz = null;
            try {
                expectedJQuiz = binary2JQuiz.Convert(expectedBin);
            } catch (Exception ex) {
                Assert.Fail($"Exception BinaryFormat -> JQuiz with {node.Path}\n{ex}");
            }

            // JQuiz -> NCF of Pos
            var jquiz2Po = new JQuiz2Po();
            NodeContainerFormat expectedPo = null;
            try {
                expectedPo = jquiz2Po.Convert(expectedJQuiz);
            } catch (Exception ex) {
                Assert.Fail($"Exception JQuiz -> Po with {node.Path}\n{ex}");
            }

            // Po -> JQuiz
            JQuiz actualJQuiz = null;
            try {
                actualJQuiz = jquiz2Po.Convert(expectedPo);
            } catch (Exception ex) {
                Assert.Fail($"Exception Po -> JQuiz with {node.Path}\n{ex}");
            }

            // JQuiz -> BinaryFormat
            BinaryFormat actualBin = null;
            try {
                actualBin = binary2JQuiz.Convert(actualJQuiz);
            } catch (Exception ex) {
                Assert.Fail($"Exception Stage -> JQuiz with {node.Path}\n{ex}");
            }

            // Comparing Binaries
            Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"JQuiz is not identical: {node.Path}");
        }
    }
}
