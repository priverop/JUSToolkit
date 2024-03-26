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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using JUSToolkit.Containers;
using JUSToolkit.Containers.Converters;
using JUSToolkit.Graphics.Converters;
using NUnit.Framework;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Tests.Containers
{
    [TestFixture]
    public class Alar3Tests
    {
        public static IEnumerable<TestCaseData> GetAlar3Files()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Containers");
            string listPath = Path.Combine(basePath, "alar3.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]))
                    .SetName($"({data[0]}, {data[1]})"));
        }

        public static IEnumerable<TestCaseData> GetAlar3InsertionFiles()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Containers");
            string listPath = Path.Combine(basePath, "alar3insertion.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]))
                    .SetName($"({data[0]}, {data[1]})"));
        }

        [TestCaseSource(nameof(GetAlar3Files))]
        public void DeserializeAlar3(string infoPath, string alarPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(alarPath);
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);

            var expected = NodeContainerInfo.FromYaml(infoPath);

            using var alar = NodeFactory.FromFile(alarPath, FileOpenMode.Read);

            alar.Invoking(n => n.TransformWith<Binary2Alar3>()).Should().NotThrow();
            alar.Should().MatchInfo(expected);
        }

        [TestCaseSource(nameof(GetAlar3Files))]
        public void TwoWaysIdenticalAlar3Stream(string infoPath, string alarPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(alarPath);
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);

            using Node node = NodeFactory.FromFile(alarPath, FileOpenMode.Read);

            Alar3 alar = node.GetFormatAs<IBinary>.ConvertWith(new Binary2Alar3());
            BinaryFormat generatedStream = alar.ConvertWith(new Alar3ToBinary());

            generatedStream.Stream.Length.Should().Be(node.Stream!.Length);
            generatedStream.Stream.Compare(node.Stream).Should().BeTrue();
        }

        [TestCaseSource(nameof(GetAlar3InsertionFiles))]
        public void InsertingAlar3Identical(string alarPath, string dirPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(alarPath);

            using Node alarOriginal = NodeFactory.FromFile(alarPath, FileOpenMode.Read);
            using Node fileOriginal = NodeFactory.FromDirectory(dirPath);

            Alar3 alar = alarOriginal.GetFormatAs<IBinary>.ConvertWith(new Binary2Alar3());
            alar.InsertModification(fileOriginal);
            BinaryFormat generatedStream = alar.ConvertWith(new Alar3ToBinary());

            generatedStream.Stream.Length.Should().Be(alarOriginal.Stream!.Length);
            generatedStream.Stream.Compare(alarOriginal.Stream).Should().BeTrue();
        }

        [Test]
        public void Alar3ReplaceStreamTest()
        {
            var streamA = new DataStream();
            streamA.Write(new byte[] { 1, 2, 3 }, 0, 3);

            var alarFile = new Alar3File(streamA);

            var streamB = new DataStream();
            streamB.Write(new byte[] { 2, 3, 4 }, 0, 3);

            alarFile.ReplaceStream(streamB);

            Assert.AreEqual(3, alarFile.Size);
            alarFile.Stream.Compare(streamB).Should().BeTrue();
            alarFile.Stream.Compare(streamA).Should().BeFalse();
        }

        [Test]
        public void Alar3InsertNodesTest()
        {
            var totalFiles = 4;

            // Alar3 con 4 AlarFiles (offset de 5 en 5, size 5 todos)
            var alar = new Alar3((uint)totalFiles);
            for (int i = 0; i < totalFiles; i++)
            {
                var child = new Alar3File(new DataStream(new MemoryStream(new byte[] { (byte)i, (byte)(i + 1), (byte)(i + 2), (byte)(i + 3), (byte)(i + 4) })))
                {
                    Size = 5,
                    Offset = (uint)(i * 5),
                };
                alar.Root.Add(new Node("child" + i, child));
            }

            // Node con 1 AlarFile, será el segundo (offset 5, size 10)
            var modifiedChild1 = new Alar3File(new DataStream(new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })))
            {
                Size = 10,
                Offset = 5,
            };
            var modifiedNode = new Node("child1", modifiedChild1);
            var modifiedFiles = new NodeContainerFormat();
            modifiedFiles.Root.Add(modifiedNode);

            // Comprobamos que todo se haya creado bien:
            // Cuántos hijos tiene el Alar3
            Assert.AreEqual(totalFiles, alar.Root.Children.Count());

            // Tamaños
            Assert.AreEqual(5, alar.Root.Children[0].GetFormatAs<Alar3File>().Size);
            Assert.AreEqual(5, alar.Root.Children[1].GetFormatAs<Alar3File>().Size);
            Assert.AreEqual(5, alar.Root.Children[2].GetFormatAs<Alar3File>().Size);
            Assert.AreEqual(5, alar.Root.Children[3].GetFormatAs<Alar3File>().Size);

            // Offsets
            Assert.AreEqual(0, alar.Root.Children[0].GetFormatAs<Alar3File>().Offset);
            Assert.AreEqual(5, alar.Root.Children[1].GetFormatAs<Alar3File>().Offset);
            Assert.AreEqual(10, alar.Root.Children[2].GetFormatAs<Alar3File>().Offset);
            Assert.AreEqual(15, alar.Root.Children[3].GetFormatAs<Alar3File>().Offset);

            // Si el Nodo getFormat . Size está OK
            var child2 = modifiedFiles.Root.Children[0].GetFormatAs<Alar3File>();
            Assert.AreEqual(10, child2.Size);
            Assert.AreEqual(5, child2.Offset);
            Assert.AreEqual(1, modifiedFiles.Root.Children.Count());

            // Insertamos el Nodo con InsertModification
            alar.InsertModification(modifiedFiles.Root);

            // Comprobamos los ficheros totales
            Assert.AreEqual(totalFiles, alar.Root.Children.Count());

            // Comprobamos los tamaños
            Assert.AreEqual(5, alar.Root.Children[0].GetFormatAs<Alar3File>().Size);
            Assert.AreEqual(10, alar.Root.Children[1].GetFormatAs<Alar3File>().Size);
            Assert.AreEqual(5, alar.Root.Children[2].GetFormatAs<Alar3File>().Size);
            Assert.AreEqual(5, alar.Root.Children[3].GetFormatAs<Alar3File>().Size);

            // Comprobamos el tema de los offsets (0, 5, 15, 20)
            Assert.AreEqual(0, alar.Root.Children[0].GetFormatAs<Alar3File>().Offset);
            Assert.AreEqual(5, alar.Root.Children[1].GetFormatAs<Alar3File>().Offset);
            Assert.AreEqual(15, alar.Root.Children[2].GetFormatAs<Alar3File>().Offset);
            Assert.AreEqual(20, alar.Root.Children[3].GetFormatAs<Alar3File>().Offset);
        }
    }
}
