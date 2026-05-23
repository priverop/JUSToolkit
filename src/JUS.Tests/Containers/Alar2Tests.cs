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
using NUnit.Framework;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tests.Containers
{
    [TestFixture]
    public class Alar2Tests
    {
        public static IEnumerable<TestCaseData> GetAlar2Files()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Containers");
            string listPath = Path.Combine(basePath, "alar2.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]))
                    .SetName($"({data[0]}, {data[1]})"));
        }

        public static IEnumerable<TestCaseData> GetAlar2InsertionFiles()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Containers");
            string listPath = Path.Combine(basePath, "alar2insertion.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]))
                    .SetName($"({data[0]}, {data[1]})"));
        }

        [TestCaseSource(nameof(GetAlar2Files))]
        public void DeserializeAlar2(string infoPath, string alarPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(alarPath);
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);

            var expected = NodeContainerInfo.FromYaml(infoPath);

            using var alar = NodeFactory.FromFile(alarPath, FileOpenMode.Read);

            var act = () => alar.TransformWith<Binary2Alar2>();
            act.Should().NotThrow();
            alar.Should().MatchInfo(expected);
        }

        [TestCaseSource(nameof(GetAlar2Files))]
        public void TwoWaysIdenticalAlar2Stream(string infoPath, string alarPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(alarPath);

            using Node node = NodeFactory.FromFile(alarPath, FileOpenMode.Read);

            Alar2 alar = node.GetFormatAs<IBinary>()!.ConvertWith(new Binary2Alar2());
            BinaryFormat generatedStream = alar.ConvertWith(new Alar2ToBinary());

            generatedStream.Stream.Length.Should().Be(node.Stream!.Length);
            generatedStream.Stream.Compare(node.Stream).Should().BeTrue();
        }

        [TestCaseSource(nameof(GetAlar2InsertionFiles))]
        public void InsertingAlar2Identical(string alarPath, string dirPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(alarPath);

            using Node alarOriginal = NodeFactory.FromFile(alarPath, FileOpenMode.Read);
            using Node fileOriginal = NodeFactory.FromDirectory(dirPath);

            Alar2 alar = new Binary2Alar2().Convert(alarOriginal.GetFormatAs<IBinary>()!);
            alar.InsertModification(fileOriginal.GetFormatAs<NodeContainerFormat>()!);
            BinaryFormat generatedStream = alar.ConvertWith(new Alar2ToBinary());

            generatedStream.Stream.Length.Should().Be(alarOriginal.Stream!.Length);
            generatedStream.Stream.Compare(alarOriginal.Stream).Should().BeTrue();
        }

        [Test]
        public void Alar2InsertNodesTest()
        {
            const int totalFiles = 4;

            // Alar2 con 4 AlarFiles (offset de 5 en 5, size 5 todos)
            var alar = new Alar2();
            for (int i = 0; i < totalFiles; i++) {
                var child = new Alar2File(DataStreamFactory.FromArray([(byte)i, (byte)(i + 1), (byte)(i + 2), (byte)(i + 3), (byte)(i + 4)]));
                alar.Root.Add(new Node("child" + i, child));
            }

            // Node con 1 AlarFile, será el segundo (offset 5, size 10)
            var modifiedChild1 = new Alar2File(DataStreamFactory.FromArray([1, 2, 3, 4, 5, 6, 7, 8, 9, 10]));
            var modifiedNode = new Node("child1", modifiedChild1);
            var modifiedFiles = new NodeContainerFormat();
            modifiedFiles.Root.Add(modifiedNode);

            // Comprobamos que todo se haya creado bien:
            // Cuántos hijos tiene el Alar2
            Assert.That(alar.Root.Children.Count, Is.EqualTo(totalFiles));

            // Si el Nodo getFormat . Size está OK
            var child2 = modifiedFiles.Root.Children[0].GetFormatAs<Alar2File>()!;
            Assert.That(modifiedFiles.Root.Children.Count, Is.EqualTo(1));

            // Insertamos el Nodo con InsertModification
            alar.InsertModification(modifiedFiles);

            // Comprobamos los ficheros totales
            Assert.That(alar.Root.Children.Count, Is.EqualTo(totalFiles));
        }
    }
}
