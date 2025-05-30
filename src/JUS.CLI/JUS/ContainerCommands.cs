// Copyright (c) 2022 Priverop

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
using System;
using System.IO;
using JUSToolkit.Containers;
using JUSToolkit.Containers.Converters;
using JUSToolkit.Graphics.Converters;
using JUSToolkit.Utils;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.CLI.JUS
{
    /// <summary>
    /// Commands related to container files.
    /// </summary>
    public static class ContainerCommands
    {
        /// <summary>
        /// Export all the files from an Alar container.
        /// </summary>
        /// <param name="container">The path to the alar file.</param>
        /// <param name="output">The output directory.</param>
        public static void Export(string container, string output)
        {
            Console.WriteLine("Exporting Alar");
            Console.WriteLine("Container: " + container);

            PathValidator.ValidateFile(container);

            Node files = NodeFactory.FromFile(container)
                .TransformWith<LzssDecompression>() ?? throw new FormatException("Invalid container file");

            Version alarVersion = Identifier.GetAlarVersion(files.Stream);

            // ToDo: In the future we need to encapsulate this
            if (alarVersion.Major == 3) {
                files.TransformWith<Binary2Alar3>();
            } else if (alarVersion.Major == 2) {
                files.TransformWith<Binary2Alar2>();
            }

            foreach (Node node in Navigator.IterateNodes(files)) {
                if (!node.IsContainer) {
                    // Path.Combine ignores the relative path if there is an absolute path
                    // so we remove the first slash of the node.Path
                    string outputFile = Path.Combine(output, node.Path[1..]);
                    node.Stream.WriteTo(outputFile);
                }
            }

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Export all the files from the Alar3 container.
        /// </summary>
        /// <param name="container">The path to the alar3 file.</param>
        /// <param name="output">The output directory.</param>
        public static void ExportAlar3(string container, string output)
        {
            Console.WriteLine("Exporting Alar3");
            Console.WriteLine("Container: " + container);

            PathValidator.ValidateFile(container);

            Node files = NodeFactory.FromFile(container)
                .TransformWith<LzssDecompression>()
                .TransformWith<Binary2Alar3>() ?? throw new FormatException("Invalid container file");

            foreach (Node node in Navigator.IterateNodes(files)) {
                if (!node.IsContainer) {
                    // Path.Combine ignores the relative path if there is an absolute path
                    // so we remove the first slash of the node.Path
                    string outputFile = Path.Combine(output, node.Path[1..]);
                    node.Stream.WriteTo(outputFile);
                }
            }

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Export all the files from the Alar2 container.
        /// </summary>
        /// <param name="container">The path to the alar2 file.</param>
        /// <param name="output">The output directory.</param>
        public static void ExportAlar2(string container, string output)
        {
            Console.WriteLine("Exporting Alar2");
            Console.WriteLine("Container: " + container);

            PathValidator.ValidateFile(container);

            Node files = NodeFactory.FromFile(container)
                .TransformWith<LzssDecompression>()
                .TransformWith<Binary2Alar2>() ?? throw new FormatException("Invalid container file");

            foreach (Node node in Navigator.IterateNodes(files)) {
                if (!node.IsContainer) {
                    // Path.Combine ignores the relative path if there is an absolute path
                    // so we remove the first slash of the node.Path
                    string outputFile = Path.Combine(output, node.Path[1..]);
                    node.Stream.WriteTo(outputFile);
                }
            }

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Import files into an Alar container.
        /// </summary>
        /// <param name="container">The path to the original alar file.</param>
        /// <param name="input">The path to the directory of the files we want to add.</param>
        /// <param name="output">The output directory.</param>
        public static void Import(string container, string input, string output)
        {
            Console.WriteLine("Importing Alar");
            Console.WriteLine("Container: " + container);
            Console.WriteLine("Input files from: " + input);

            PathValidator.ValidateFile(container);
            PathValidator.ValidateDirectory(input);

            Node originalAlar = NodeFactory.FromFile(container) ?? throw new FormatException("Invalid container file");

            bool originalIsCompressed = CompressionUtils.IsCompressed(originalAlar);

            if (originalIsCompressed) {
                _ = originalAlar.TransformWith<LzssDecompression>();
            }

            Version alarVersion = Identifier.GetAlarVersion(originalAlar.Stream);

            var binary = new BinaryFormat();
            var filesToInsert = new NodeContainerFormat();
            filesToInsert.Root.Add(NodeFactory.FromDirectory(input).Children);

            if (alarVersion.Major == 3) {
                Alar3 alar = originalAlar.TransformWith<Binary2Alar3>()
                .GetFormatAs<Alar3>();
                alar.InsertModification(filesToInsert);
                binary = alar.ConvertWith(new Alar3ToBinary());
            } else if (alarVersion.Major == 2) {
                Alar2 alar = originalAlar.TransformWith<Binary2Alar2>()
                .GetFormatAs<Alar2>();
                alar.InsertModification(filesToInsert);
                binary = alar.ConvertWith(new Alar2ToBinary());
            }

            binary = originalIsCompressed ?
                new LzssCompression().Convert(binary) :
                binary;

            binary.Stream.WriteTo(Path.Combine(output, "imported_" + Path.GetFileName(container)));

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Import files into an Alar3 container.
        /// </summary>
        /// <param name="container">The path to the original alar3 file.</param>
        /// <param name="input">The path to the directory of the files we want to add.</param>
        /// <param name="output">The output directory.</param>
        public static void ImportAlar3(string container, string input, string output)
        {
            Console.WriteLine("Importing Alar3");
            Console.WriteLine("Container: " + container);
            Console.WriteLine("Input files from: " + input);

            PathValidator.ValidateFile(container);
            PathValidator.ValidateDirectory(input);

            Alar3 alar = NodeFactory.FromFile(container)
                .TransformWith<Binary2Alar3>()
                .GetFormatAs<Alar3>() ?? throw new FormatException("Invalid container file");

            var filesToInsert = new NodeContainerFormat();
            Node factory = NodeFactory.FromDirectory(input);
            filesToInsert.Root.Add(factory);

            alar.InsertModification(filesToInsert);

            using BinaryFormat binary = alar.ConvertWith(new Alar3ToBinary());
            binary.Stream.WriteTo(Path.Combine(output, "imported_" + Path.GetFileName(container)));

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Import files into an Alar2 container.
        /// </summary>
        /// <param name="container">The path to the original alar2 file.</param>
        /// <param name="input">The path to the directory of the files we want to add.</param>
        /// <param name="output">The output directory.</param>
        public static void ImportAlar2(string container, string input, string output)
        {
            Console.WriteLine("Importing Alar2");
            Console.WriteLine("Container: " + container);
            Console.WriteLine("Input files from: " + input);

            PathValidator.ValidateFile(container);
            PathValidator.ValidateDirectory(input);

            Alar2 alar = NodeFactory.FromFile(container)
                .TransformWith<Binary2Alar2>()
                .GetFormatAs<Alar2>() ?? throw new FormatException("Invalid container file");

            var filesToInsert = new NodeContainerFormat();
            Node factory = NodeFactory.FromDirectory(input);
            filesToInsert.Root.Add(factory);

            alar.InsertModification(filesToInsert);

            using BinaryFormat binary = alar.ConvertWith(new Alar2ToBinary());
            binary.Stream.WriteTo(Path.Combine(output, "imported_" + Path.GetFileName(container)));

            Console.WriteLine("Done!");
        }
    }
}
