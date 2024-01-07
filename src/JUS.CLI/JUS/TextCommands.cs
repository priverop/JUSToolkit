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
using JUSToolkit.Texts.Converters;
using JUSToolkit.Texts.Formats;
using JUSToolkit.Utils;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUSToolkit.CLI.JUS
{
    /// <summary>
    /// Commands related to text files.
    /// </summary>
    public static class TextCommands
    {
        private const string TextConvertersNamespace = "JUSToolkit.Texts.Converters.";

        /// <summary>
        /// Export a .bin file to a .Po file.
        /// </summary>
        /// <param name="bin">The path to the bin file.</param>
        /// <param name="output">The output directory.</param>
        public static void Export(string bin, string output)
        {
            using Node binNode = NodeFactory.FromFile(bin, FileOpenMode.Read) ?? throw new FormatException("Invalid bin file");

            ExportBin(binNode, output);

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Export a folder full of .bin files to .Po.
        /// </summary>
        /// <param name="directory">The directory with the bin files.</param>
        /// <param name="output">The output directory.</param>
        public static void BatchExport(string directory, string output)
        {
            Node inputFiles = NodeFactory.FromDirectory(directory);
            Console.WriteLine(inputFiles.Children.Count.ToString() + " files to transform.");

            foreach (Node file in inputFiles.Children) {
                Console.WriteLine(file.Name);
                ExportBin(file, output);
            }
        }

        /// <summary>
        /// Auxiliary function to export a bin file to .po.
        /// </summary>
        /// <param name="binNode">The node with the bin file.</param>
        /// <param name="output">The output directory.</param>
        private static void ExportBin(Node binNode, string output)
        {
            // Detect format
            string binFormatName = TextIdentifier.GetTextFormat(binNode.Name);
            Console.WriteLine("File Name: " + binNode.Name + " - File Format: " + binFormatName);

            string converterName = TextConvertersNamespace + "Binary2" + binFormatName;
            string converterPoName = TextConvertersNamespace + binFormatName + "2Po";

            // Binary -> TextFormat
            var binFormat = (IFormat)ConvertFormat.With(FormatDiscovery.GetConverter(converterName), binNode.Format!);

            // If instead of Po we get a Container
            if (binFormatName == "JQuiz") {
                var container = (NodeContainerFormat)ConvertFormat.With(FormatDiscovery.GetConverter(converterPoName), binFormat);
                foreach (Node quiz in container.Root.Children) {
                    quiz.Stream.WriteTo(Path.Combine(output, quiz.Name));
                }
            } else {
                // TextFormat -> Po
                var poFormat = (IFormat)ConvertFormat.With(FormatDiscovery.GetConverter(converterPoName), binFormat);

                // Po -> Binary
                var poBinaryFormat = (BinaryFormat)ConvertFormat.With<Po2Binary>(poFormat);

                string outputFile = Path.Combine(output, binNode.Name + ".po");
                poBinaryFormat.Stream.WriteTo(outputFile);
            }
        }

        /// <summary>
        /// Import a .po file to a .bin file.
        /// </summary>
        /// <param name="po">The path to the po file.</param>
        /// <param name="output">The output directory.</param>
        public static void Import(string po, string output)
        {
            using Node poNode = NodeFactory.FromFile(po, FileOpenMode.Read)
                .TransformWith<Binary2Po>() ?? throw new FormatException("Invalid po file");

            string cleanFileName = Path.GetFileNameWithoutExtension(poNode.Name);

            // Detect format
            string binFormatName = TextIdentifier.GetTextFormat(cleanFileName);
            Console.WriteLine("File Format: " + binFormatName);

            string converterPoName = TextConvertersNamespace + binFormatName + "2Po";
            string converterName = TextConvertersNamespace + "Binary2" + binFormatName;

            // Po -> Text Format
            var textFormat = (IFormat)ConvertFormat.With(FormatDiscovery.GetConverter(converterPoName), poNode.Format!);

            // Text Format -> Binary
            var binaryFormat = (BinaryFormat)ConvertFormat.With(FormatDiscovery.GetConverter(converterName), textFormat);

            string outputFile = Path.Combine(output, cleanFileName);
            binaryFormat.Stream.WriteTo(outputFile);
            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Import the jquiz container to a .bin file.
        /// </summary>
        /// <param name="container">The path to the po file.</param>
        /// <param name="output">The output directory.</param>
        public static void ImportJQuiz(string container, string output)
        {
            Node inputFiles = NodeFactory.FromDirectory(container, "*.po") ?? throw new FormatException("Invalid container file");

            inputFiles.SortChildren((x, y) => string.Compare(x.Name, y.Name, StringComparison.CurrentCulture));

            JQuiz jquiz = inputFiles
                .TransformWith<JQuiz2Po>()
                .GetFormatAs<JQuiz>();

            using var binary = (BinaryFormat)ConvertFormat.With<Binary2JQuiz>(jquiz);

            binary.Stream.WriteTo(Path.Combine(output, "imported_jquiz.bin"));
            Console.WriteLine("Done!");
        }
    }
}
