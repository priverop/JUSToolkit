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
    public static class TextExportCommands
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
        /// Export a folder full of .bin files to a single .po file.
        /// </summary>
        /// <param name="directory">The directory with the bin files.</param>
        /// <param name="pdeck">If the files are PDeck files.</param>
        /// <param name="output">The output directory.</param>
        public static void DeckExport(string directory, bool pdeck, string output)
        {
            Console.WriteLine($"Exporting {directory}");

            // Ensure the path does not end with a directory separator
            directory = directory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Get the last directory or file name from the path
            string lastDirectory = Path.GetFileName(directory);

            Node inputFiles = NodeFactory.FromDirectory(directory, "*.bin");
            inputFiles.SortChildren((x, y) => string.Compare(x.Name, y.Name, StringComparison.CurrentCulture));
            Console.WriteLine(inputFiles.Children.Count.ToString() + " files to transform.");

            IConverter binConverter = pdeck ? new Binary2PDeck() : new Binary2Deck();
            IConverter poConverter = pdeck ? new PDeck2Po() : new Deck2Po();

            // NodeContainerFormat with all the (P)Deck files
            var container = new Node("parent", new NodeContainerFormat());

            foreach (Node file in inputFiles.Children) {
                Console.WriteLine(file.Name);

                // BinaryFormat -> TextFormat
                container.Add(new Node(file).TransformWith(binConverter));
            }

            // TextFormat -> Po
            Node poFormat = container.TransformWith(poConverter);

            // Po -> Binary
            BinaryFormat poBinaryFormat = new Po2Binary().Convert(poFormat.GetFormatAs<Po>());

            string outputFile = Path.Combine(output, $"deck-{lastDirectory}.po");
            poBinaryFormat.Stream.WriteTo(outputFile);
            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Export a folder full of .bin files to .Po.
        /// </summary>
        /// <param name="directory">The directory with the bin files.</param>
        /// <param name="output">The output directory.</param>
        public static void BatchExport(string directory, string output)
        {
            Node inputFiles = NodeFactory.FromDirectory(directory, "*.bin");
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
            string filename = binNode.Name;

            // Detect converters
            Type[] formatConverters = TextIdentifier.GetTextFormat(filename);
            Type binConverterName = formatConverters[0];
            Type poConverterName = formatConverters[1];
            Console.WriteLine("File Name: " + filename + " - Bin Converter: " + binConverterName + " - Po Converter: " + poConverterName);

            // BinaryFormat -> TextFormat
            object textFormat = ConvertFormat.With(binConverterName, binNode.Format!);

            // If instead of Po we get a Container
            if (binConverterName.ToString() == "JUSToolkit.Texts.Converters.Binary2JQuiz") {
                var container = (NodeContainerFormat)ConvertFormat.With(poConverterName, textFormat);
                foreach (Node quiz in container.Root.Children) {
                    quiz.Stream.WriteTo(Path.Combine(output, quiz.Name));
                }
            } else {
                // TextFormat -> Po
                var poFormat = (Po)ConvertFormat.With(poConverterName, textFormat);

                // Po -> Binary
                BinaryFormat poBinaryFormat = new Po2Binary().Convert(poFormat);

                string outputFile = Path.Combine(output, filename + ".po");
                poBinaryFormat.Stream.WriteTo(outputFile);
            }
        }
    }
}
