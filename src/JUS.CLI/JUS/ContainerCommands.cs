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

using JUS.Tool;
using JUS.Tool.Containers.Converters;
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
                .TransformWith<Binary2Alar>();

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
        /// <param name="containerPath">The path to the original alar file.</param>
        /// <param name="inputPath">The path to the directory of the files we want to add.</param>
        /// <param name="output">The output directory.</param>
        public static void Import(string containerPath, string inputPath, string output)
        {
            Console.WriteLine("Importing Alar");
            Console.WriteLine("Container: " + containerPath);
            Console.WriteLine("Input files from: " + inputPath);

            PathValidator.ValidateFile(containerPath);
            PathValidator.ValidateDirectory(inputPath);

            using Node container = NodeFactory.FromFile(containerPath)
                .TransformWith(new Binary2Alar());

            using Node inputRoot = NodeFactory.FromDirectory(inputPath, "*", "root", true, FileOpenMode.Read);
            container.ReplaceBinaryChildren(inputRoot);

            string outputFilePath = Path.Combine(output, Path.GetFileName(containerPath));
            container.TransformWith(new AlarToBinary())
                .Stream.WriteTo(outputFilePath);

            Console.WriteLine("Done!");
        }
    }
}
