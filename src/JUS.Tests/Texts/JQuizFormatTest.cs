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
    public class JQuizFormatTests
    {
        private string resPath;

        // [SetUp]
        // public void Setup()
        // {
        //     string programDir = AppDomain.CurrentDomain.BaseDirectory;
        //     resPath = Path.GetFullPath(programDir + "/../../../" + "Resources/Texts/JQuiz/");

        //     Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        // }

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

        // public static IEnumerable<TestCaseData> GetJQuizInsertionFiles()
        // {
        //     string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Containers");
        //     string listPath = Path.Combine(basePath, "JQuizinsertion.txt");
        //     return TestDataBase.ReadTestListFile(listPath)
        //         .Select(line => line.Split(','))
        //         .Select(data => new TestCaseData(
        //             Path.Combine(basePath, data[0]),
        //             Path.Combine(basePath, data[1]))
        //             .SetName($"({data[0]}, {data[1]})"));
        // }

        [TestCaseSource(nameof(GetJQuizFiles))]
        public void DeserializeJQuiz(string infoPath, string jquizPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(jquizPath);
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);

            var expected = NodeContainerInfo.FromYaml(infoPath);

            using var jquiz = NodeFactory.FromFile(jquizPath, FileOpenMode.Read);

            jquiz.Invoking(n => n.TransformWith<Binary2JQuiz>()).Should().NotThrow();
            jquiz.Should().MatchInfo(expected);
        }

        [TestCaseSource(nameof(GetJQuizFiles))]
        public void TwoWaysIdenticalJQuizStream(string infoPath, string jquizPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(jquizPath);

            using Node node = NodeFactory.FromFile(jquizPath, FileOpenMode.Read);

            var jquiz = (JQuiz)ConvertFormat.With<Binary2JQuiz>(node.Format!);
            var generatedStream = (BinaryFormat)ConvertFormat.With<Binary2JQuiz>(jquiz);

            generatedStream.Stream.Length.Should().Be(node.Stream!.Length);
            generatedStream.Stream.Compare(node.Stream).Should().BeTrue();
        }

        // [TestCaseSource(nameof(GetJQuizInsertionFiles))]
        // public void InsertingJQuizIdentical(string jquizPath, string filePath)
        // {
        //     TestDataBase.IgnoreIfFileDoesNotExist(jquizPath);
        //     TestDataBase.IgnoreIfFileDoesNotExist(filePath);

        //     Assert.Ignore();

        //     using Node jquizOriginal = NodeFactory.FromFile(jquizPath, FileOpenMode.Read);
        //     using Node fileOriginal = NodeFactory.FromFile(filePath, FileOpenMode.Read);

        //     var jquiz = (JQuiz)ConvertFormat.With<Binary2JQuiz>(jquizOriginal.Format!);
        //     jquiz.InsertModification(fileOriginal);
        //     var generatedStream = (BinaryFormat)ConvertFormat.With<Binary2JQuiz>(jquiz);

        //     generatedStream.Stream.Length.Should().Be(jquizOriginal.Stream!.Length);
        //     generatedStream.Stream.Compare(jquizOriginal.Stream).Should().BeTrue();
        // }

        // [Test]
        // public void JQuizTest()
        // {
        //     foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
        //         using (var node = NodeFactory.FromFile(filePath)) {
        //             // BinaryFormat -> JQuiz
        //             var expectedBin = node.GetFormatAs<BinaryFormat>();
        //             var binary2JQuiz = new Binary2JQuiz();
        //             JQuiz expectedJQuiz = null;
        //             try {
        //                 expectedJQuiz = binary2JQuiz.Convert(expectedBin);
        //             } catch (Exception ex) {
        //                 Assert.Fail($"Exception BinaryFormat -> JQuiz with {node.Path}\n{ex}");
        //             }

        //             // JQuiz -> Po
        //             var jquiz2Po = new JQuiz2Po();
        //             Po expectedPo = null;
        //             try {
        //                 expectedPo = jquiz2Po.Convert(expectedJQuiz);
        //             } catch (Exception ex) {
        //                 Assert.Fail($"Exception JQuiz -> Po with {node.Path}\n{ex}");
        //             }

        //             // Po -> JQuiz
        //             JQuiz actualJQuiz = null;
        //             try {
        //                 actualJQuiz = jquiz2Po.Convert(expectedPo);
        //             } catch (Exception ex) {
        //                 Assert.Fail($"Exception Po -> JQuiz with {node.Path}\n{ex}");
        //             }

        //             // JQuiz -> BinaryFormat
        //             BinaryFormat actualBin = null;
        //             try {
        //                 actualBin = binary2JQuiz.Convert(actualJQuiz);
        //             } catch (Exception ex) {
        //                 Assert.Fail($"Exception Stage -> JQuiz with {node.Path}\n{ex}");
        //             }

        //             // Comparing Binaries
        //             Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"JQuiz is not identical: {node.Path}");
        //         }
        //     }
        // }
    }
}
