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
using System.Linq;
using System.Text.RegularExpressions;
using JUSToolkit.BatchConverters;
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
    /// Commands using Containers and Graphics. Batch import, export...
    /// </summary>
    public static class BatchCommands
    {
        private static readonly Dictionary<string, int> DemoImages = new Dictionary<string, int>() {
            { "_03.dig", 0 },
            { "_05.dig", 1 },
            { "_07.dig", 2 },
            { "_09.dig", 3 },
        };

        /// <summary>
        /// Export PNG files from the .dig files of an Alar container. Includes special demo files.
        /// </summary>
        /// <param name="container">The path to the alar file.</param>
        /// <param name="output">The output directory.</param>
        public static void ExportAlarDig2Png(string container, string output)
        {
            Node originalAlar = NodeFactory.FromFile(container) ?? throw new FormatException("Invalid container file");

            var alar2png = new Alar2Png(DemoImages);

            _ = container == "demo.aar" ? originalAlar
                    .TransformWith(alar2png)
                : originalAlar.TransformWith<Alar2Png>();

            NodeContainerFormat result = originalAlar
                .GetFormatAs<NodeContainerFormat>();

            foreach (Node image in result.Root.Children) {
                image.Stream.WriteTo(Path.Combine(output, image.Name + ".png"));
            }

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Export PNG files from the dtx files of an Alar container.
        /// </summary>
        /// <param name="container">The path to the alar file.</param>
        /// <param name="output">The output directory.</param>
        public static void ExportAlarDtx2Png(string container, string output)
        {
            Node originalAlar = NodeFactory.FromFile(container)
                            .TransformWith<LzssDecompression>() ?? throw new FormatException("Invalid container file");

            Version alarVersion = Identifier.GetAlarVersion(originalAlar.Stream);

            // ToDo: In the future we need to encapsulate this
            if (alarVersion.Major == 3) {
                originalAlar.TransformWith<Binary2Alar3>();
            } else if (alarVersion.Major == 2) {
                originalAlar.TransformWith<Binary2Alar2>();
            }

            foreach (Node child in Navigator.IterateNodes(originalAlar)) {
                if (Path.GetExtension(child.Name) == ".dtx") {
                    using Node dtx3 = child
                        .TransformWith<LzssDecompression>()
                        .TransformWith<Dtx2Bitmaps>();

                    foreach (Node nodeSprite in dtx3.Children) {
                        nodeSprite.Stream.WriteTo(Path.Combine(output, $"{nodeSprite.Name}.png"));
                    }
                }
            }

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Import PNG files into an Alar3 container.
        /// </summary>
        /// <param name="container">The path to the original alar3 file.</param>
        /// <param name="input">The path to the PNG we want to insert.</param>
        /// <param name="output">The output directory.</param>
        public static void ImportPng2Alar3(string container, string input, string output)
        {
            Node originalAlar = NodeFactory.FromFile(container).TransformWith<Binary2Alar3>() ?? throw new FormatException("Invalid container file");
            Node inputPNG = NodeFactory.FromFile(input);

            string cleanName = StringFunctions.GetOriginalName(inputPNG.Name);

            var png2Alar3 = new Png2Alar3(inputPNG, cleanName + ".dig", cleanName + ".atm");

            Alar3 newAlar = originalAlar
                .TransformWith(png2Alar3)
                .GetFormatAs<Alar3>();

            using BinaryFormat binary = newAlar.ConvertWith(new Alar3ToBinary());

            binary.Stream.WriteTo(Path.Combine(output, "imported_" + originalAlar.Name));

            Console.WriteLine("Done!");
        }
    }
}
