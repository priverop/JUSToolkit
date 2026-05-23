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
using System.Reflection;
using FluentAssertions;
using JUS.Tool.Containers;
using JUS.Tool.Containers.Converters;
using JUS.Tool.Graphics.Converters;
using NUnit.Framework;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tests.Containers
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

        public static IEnumerable<TestCaseData> GetAlar3SubDirectoriesInsertionFiles()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Containers/alar3subdirectories");
            string listPath = Path.Combine(basePath, "alar3subdirectoriesinsertion.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]),
                    data[2],
                    data[3])
                    .SetName($"({data[0]}, {data[1]}, {data[2]}, {data[3]})"));
        }

        [TestCaseSource(nameof(GetAlar3Files))]
        public void DeserializeAlar3(string infoPath, string alarPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(alarPath);
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);

            var expected = NodeContainerInfo.FromYaml(infoPath);

            using var alar = NodeFactory.FromFile(alarPath, FileOpenMode.Read);

            var act = () => alar.TransformWith<Binary2Alar3>();
            act.Should().NotThrow();
            alar.Should().MatchInfo(expected);
        }

        [TestCaseSource(nameof(GetAlar3Files))]
        public void TwoWaysIdenticalAlar3Stream(string infoPath, string alarPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(alarPath);
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);

            using Node node = NodeFactory.FromFile(alarPath, FileOpenMode.Read);

            Alar3 alar = node.GetFormatAs<IBinary>()!.ConvertWith(new Binary2Alar3());
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

            Alar3 alar = alarOriginal.GetFormatAs<IBinary>()!.ConvertWith(new Binary2Alar3());
            alar.InsertModification(fileOriginal.GetFormatAs<NodeContainerFormat>()!);
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

            alarFile.Stream = streamB;

            Assert.That(alarFile.Stream.Length, Is.EqualTo(3));
            alarFile.Stream.Compare(streamB).Should().BeTrue();
            alarFile.Stream.Compare(streamA).Should().BeFalse();
        }

        [Test]
        public void InsertNodes()
        {
            const int totalFiles = 4;

            // Alar3 con 4 AlarFiles (offset de 5 en 5, size 5 todos)
            var alar = new Alar3();
            for (int i = 0; i < totalFiles; i++) {
                // Creamos un fichero de 5bytes.
                var child = new Alar3File(DataStreamFactory.FromArray([(byte)i, (byte)(i + 1), (byte)(i + 2), (byte)(i + 3), (byte)(i + 4)]));
                alar.Root.Add(new Node("child" + i, child));
            }

            // Node con 1 AlarFile, será el segundo (offset 5, size 10)
            var newStream = DataStreamFactory.FromArray([1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);
            var modifiedChild1 = new Alar3File(newStream);
            var modifiedNode = new Node("child1", modifiedChild1);
            var modifiedFiles = new NodeContainerFormat();
            modifiedFiles.Root.Add(modifiedNode);

            // Comprobamos que todo se haya creado bien:
            // Cuántos hijos tiene el Alar3
            Assert.That(alar.Root.Children.Count, Is.EqualTo(totalFiles));

            // Si el Nodo getFormat . Size está OK
            var child2 = modifiedFiles.Root.Children[0].GetFormatAs<Alar3File>()!;
            Assert.That(modifiedFiles.Root.Children.Count, Is.EqualTo(1));

            // Insertamos el Nodo con InsertModification
            alar.InsertModification(modifiedFiles);

            // Comprobamos los ficheros totales
            Assert.That(alar.Root.Children.Count, Is.EqualTo(totalFiles));

            // Comprobamos el contenido del fichero insertado
            Assert.That(alar.Root.Children[1].Stream, Is.EqualTo(newStream));
        }

        // Inserting Nodes in a ALAR3 with subdirectories. What if the same node.Name is in two different subdirectories of the same ALAR3?
        [TestCaseSource(nameof(GetAlar3SubDirectoriesInsertionFiles))]
        public void ReplacingNodesWithPath(string alarPath, string fileToInsert, string parent, string internalPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(alarPath);

            using Node alarOriginal = NodeFactory.FromFile(alarPath, FileOpenMode.Read);
            using Node fileOriginal = NodeFactory.FromFile(fileToInsert, FileOpenMode.Read);

            Alar3 alar = alarOriginal.GetFormatAs<IBinary>()!.ConvertWith(new Binary2Alar3());
            alar.InsertModification(fileOriginal, parent);

            // Tenemos que comprobar si se ha introducido correctamente
            // Obtenemos el fichero del alar3 y comprobamos el size
            Node newFile = Navigator.SearchNode(alar.Root, internalPath) ?? throw new FormatException("Node not found: " + internalPath);
            Assert.That(newFile.Stream!.Length, Is.EqualTo(fileOriginal.Stream!.Length));
        }

        // Unit test para la funcion de GetAlar3Path
        [Test]
        public void GetAlar3PathTest()
        {
            // Arrange
            Type type = typeof(Alar3ToBinary);
            MethodInfo method = type.GetMethod("GetAlar3Path", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException("Method GetAlar3Path not found");

            const string jgalaxyFilePath = "/root/data/jgalaxy/jgalaxy.aar/jgalaxy/ast_battle.aar";
            const string infodeckFilePath = "/root/data/bin/InfoDeck.aar/bin/deck/bb.bin";
            const string vscallFilePath = "/vscall.aar/vscall/obj_a.aar";
            const string komaFilePath = "/koma.aar/koma/bb_00.dtx";

            // Act
            string jgalaxyResult = (string)method.Invoke(null, new object[] { jgalaxyFilePath })!;
            string infodeckyResult = (string)method.Invoke(null, new object[] { infodeckFilePath })!;
            string vscallResult = (string)method.Invoke(null, new object[] { vscallFilePath })!;
            string komaResult = (string)method.Invoke(null, new object[] { komaFilePath })!;

            // Assert
            Assert.That(jgalaxyResult, Is.EqualTo("jgalaxy/ast_battle.aar"));
            Assert.That(infodeckyResult, Is.EqualTo("bin/deck/bb.bin"));
            Assert.That(vscallResult, Is.EqualTo("vscall/obj_a.aar"));
            Assert.That(komaResult, Is.EqualTo("koma/bb_00.dtx"));
        }
    }
}
