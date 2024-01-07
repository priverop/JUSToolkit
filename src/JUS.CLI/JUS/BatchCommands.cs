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
using System.Collections.Generic;
using System.IO;
using JUSToolkit.BatchConverters;
using JUSToolkit.Containers;
using JUSToolkit.Containers.Converters;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.CLI.JUS
{
    /// <summary>
    /// Commands using Containers and Graphics. Batch import, export...
    /// </summary>
    public static class BatchCommands
    {
        private static Dictionary<string, int> demoImages = new Dictionary<string, int>() {
            { "_03.dig", 0 },
            { "_05.dig", 1 },
            { "_07.dig", 2 },
            { "_09.dig", 3 },
        };

        /// <summary>
        /// Export PNG files from an Alar container.
        /// </summary>
        /// <param name="container">The path to the alar file.</param>
        /// <param name="output">The output directory.</param>
        public static void ExportAlar2Png(string container, string output)
        {
            var originalAlar = NodeFactory.FromFile(container);

            if (originalAlar is null) {
                throw new FormatException("Invalid container file");
            }

            _ = container == "demo.aar" ? originalAlar
                    .TransformWith<Alar2Png, Dictionary<string, int>>(demoImages)
                : originalAlar
                .TransformWith<Alar2Png>();

            NodeContainerFormat result = originalAlar
                .GetFormatAs<NodeContainerFormat>();

            foreach (var image in result.Root.Children) {
                image.Stream.WriteTo(Path.Combine(output, image.Name + ".png"));
            }

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Import PNG files into an Alar3 container.
        /// </summary>
        /// <param name="container">The path to the original alar3 file.</param>
        /// <param name="input">The path to the directory of the PNGs we want to add, with the original .dig and .atm.</param>
        /// <param name="output">The output directory.</param>
        public static void ImportPng2Alar3(string container, string input, string output)
        {
            var originalAlar = NodeFactory.FromFile(container);
            var inputFiles = NodeFactory.FromDirectory(input);

            Alar3 alar = inputFiles
                .TransformWith<Png2Alar3, Node>(originalAlar)
                .GetFormatAs<Alar3>();

            using var binary = (BinaryFormat)ConvertFormat.With<Alar3ToBinary>(alar);

            binary.Stream.WriteTo(Path.Combine(output, "imported_" + container));

            Console.WriteLine("Done!");
        }
    }
}
