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
    public static class TextImportCommands
    {
        /// <summary>
        /// Import a .po file to a .bin file.
        /// </summary>
        /// <param name="po">The path to the po file.</param>
        /// <param name="output">The output directory.</param>
        public static void Import(string po, string output)
        {
            using Node poNode = NodeFactory.FromFile(po, FileOpenMode.Read)
                .TransformWith<Binary2Po>() ?? throw new FormatException("Invalid po file");

            ImportBin(poNode, output);
            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Import a folder full of .po files to .bin.
        /// </summary>
        /// <param name="directory">The directory with the po files.</param>
        /// <param name="output">The output directory.</param>
        public static void BatchImport(string directory, string output)
        {
            Node inputFiles = NodeFactory.FromDirectory(directory, "*.po");
            Console.WriteLine(inputFiles.Children.Count.ToString() + " files to transform.");

            foreach (Node file in inputFiles.Children) {
                Console.WriteLine("Processing " + file.Name);
                ImportBin(file.TransformWith<Binary2Po>(), output);
            }
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

            using BinaryFormat binary = new Binary2JQuiz().Convert(jquiz);

            binary.Stream.WriteTo(Path.Combine(output, "imported_jquiz.bin"));
            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Import a single .po file to a .bin file.
        /// </summary>
        /// <param name="poNode">The Node with the po file in Po Format.</param>
        /// <param name="output">The output directory.</param>
        private static void ImportBin(Node poNode, string output)
        {
            string cleanFileName = Path.GetFileNameWithoutExtension(poNode.Name);

            // Detect converter
            Type[] formatConverters = TextIdentifier.GetTextFormat(cleanFileName);
            Type binConverterName = formatConverters[0];
            Type poConverterName = formatConverters[1];
            Console.WriteLine("File Name: " + cleanFileName + " - Bin Converter: " + binConverterName + " - Po Converter: " + poConverterName);

            // Po -> Text Format
            object textFormat = ConvertFormat.With(poConverterName, poNode.Format!);

            // // Text Format -> Binary
            var binaryFormat = (BinaryFormat)ConvertFormat.With(binConverterName, textFormat);

            string outputFile = Path.Combine(output, cleanFileName);
            binaryFormat.Stream.WriteTo(outputFile);
        }
    }
}
