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
using JUS.Tool.Containers;
using JUS.Tool.Containers.Converters;
using JUS.Tool.Graphics.Converters;
using JUS.Tool.Utils;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.CLI.JUS
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

            using Node files = NodeFactory.FromFile(container)
                .TransformWith<LzssDecompression>() ?? throw new FormatException("Invalid container file");

            Version alarVersion = Identifier.GetAlarVersion(files.Stream!);

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
                    node.Stream!.WriteTo(outputFile);
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

            using Node originalAlar = NodeFactory.FromFile(container) ?? throw new FormatException("Invalid container file");

            bool originalIsCompressed = CompressionUtils.IsCompressed(originalAlar);

            if (originalIsCompressed) {
                _ = originalAlar.TransformWith<LzssDecompression>();
            }

            Version alarVersion = Identifier.GetAlarVersion(originalAlar.Stream!);

            using var filesToInsert = new NodeContainerFormat();
            using Node inputDir = NodeFactory.FromDirectory(input);
            filesToInsert.Root.Add(inputDir.Children);

            BinaryFormat binary;
            if (alarVersion.Major == 3) {
                Alar3 alar = originalAlar.TransformWith<Binary2Alar3>()
                    .GetFormatAs<Alar3>()!;
                alar.InsertModification(filesToInsert);
                binary = alar.ConvertWith(new Alar3ToBinary());
            } else if (alarVersion.Major == 2) {
                Alar2 alar = originalAlar.TransformWith<Binary2Alar2>()
                    .GetFormatAs<Alar2>()!;
                alar.InsertModification(filesToInsert);
                binary = alar.ConvertWith(new Alar2ToBinary());
            } else {
                throw new FormatException($"Unsupported ALAR version: {alarVersion}");
            }

            using (binary) {
                if (originalIsCompressed) {
                    using BinaryFormat compressed = new LzssCompression().Convert(binary);
                    compressed.Stream.WriteTo(Path.Combine(output, Path.GetFileName(container)));
                } else {
                    binary.Stream.WriteTo(Path.Combine(output, Path.GetFileName(container)));
                }
            }

            Console.WriteLine("Done!");
        }
    }
}
