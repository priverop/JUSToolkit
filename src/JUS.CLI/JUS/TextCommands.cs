// Copyright (c) 2022 Pablo Rivero

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
        private static string textConvertersNamespace = "JUSToolkit.Texts.Converters.";

        /// <summary>
        /// Export a .bin file to a .Po file.
        /// </summary>
        /// <param name="bin">The path to the bin file.</param>
        /// <param name="output">The output directory.</param>
        public static void ExportText(string bin, string output)
        {
            using Node binNode = NodeFactory.FromFile(bin, FileOpenMode.Read);

            if (binNode is null) {
                throw new FormatException("Invalid bin file");
            }

            // Detect format
            string binFormatName = TextIdentifier.GetTextFormat(binNode.Name);
            Console.WriteLine("File Name: " + binNode.Name + " File Format: " + binFormatName);

            var converterName = textConvertersNamespace + "Binary2" + binFormatName;
            var converterPoName = textConvertersNamespace + binFormatName + "2Po";

            var binFormat = (IFormat)ConvertFormat.With(FormatDiscovery.GetConverter(converterName), binNode.Format!);
            var poFormat = (IFormat)ConvertFormat.With(FormatDiscovery.GetConverter(converterPoName), binFormat);
            var poBinaryFormat = (BinaryFormat)ConvertFormat.With<Po2Binary>(poFormat);

            string outputFile = Path.Combine(output, binNode.Name + ".po");
            poBinaryFormat.Stream.WriteTo(outputFile);
        }
    }
}
