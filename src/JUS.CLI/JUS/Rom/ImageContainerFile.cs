// Copyright (c) 2024 Priverop

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
using System.Text.RegularExpressions;
using JUSToolkit.BatchConverters;
using JUSToolkit.Containers;
using JUSToolkit.Containers.Converters;
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using JUSToolkit.Utils;
using Texim.Compressions.Nitro;
using Texim.Formats;
using Texim.Images;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.CLI.JUS.Rom
{
    /// <summary>
    /// Strategy Pattern: Interface for rom importing logic.
    /// </summary>
    public class ImageContainerFile : IFileImportStrategy
    {
        private static readonly Dictionary<string, string> ContainerLocations = new() {
            { "menu-option-option.png", "/option/option.aar" },
        };

        private static readonly List<(Regex, string)> PatternList = new()
        {

            (new Regex(@"^demo-.*-.*\.bin$"), "/demo/Demo.aar"), // "{container}/bin/deck/{file.Name}"
        };

        /// <summary>
        /// Import files into the Rom.
        /// </summary>
        /// <param name="gameNode">The node of the Rom.</param>
        /// <param name="file">The input file to import.</param>
        public void Import(Node gameNode, Node file)
        {
            if (ContainerLocations.TryGetValue(file.Name, out string path)) {
                file.Name = GetOriginalName(file.Name);
                ProcessContainer(gameNode, file, path);
            } else {
                // Si no se encuentra, intenta encontrar la ruta interna usando patrones
                foreach ((Regex pattern, string containerPath) in PatternList) {
                    if (pattern.IsMatch(file.Name)) {
                        ProcessContainer(gameNode, file, containerPath);
                        return;
                    }
                }

                Console.WriteLine($"File not compatible as container: {file.Name}");
            }
        }

        private static void ProcessContainer(Node gameNode, Node file, string containerPath)
        {
            // 1 - Buscar el Alar3 Original
            Node originalAlar = Navigator.SearchNode(gameNode, $"/root/data{containerPath}")
                                .TransformWith<Binary2Alar3>();

            // 2 - Hacer el Png2Alar
            var png2Alar3 = new Png2Alar3(file);

            Alar3 newAlar = originalAlar
                .TransformWith(png2Alar3)
                .GetFormatAs<Alar3>();

            BinaryFormat newBinary = newAlar.ConvertWith(new Alar3ToBinary());

            // 3 - Sustituirlo
            originalAlar.ChangeFormat(newBinary);

            Console.WriteLine($"File replaced: /root/data{containerPath}/{file.Name}");
        }

        /// <summary>
        /// Remove the first two words separated by dashes "x-y-".
        /// </summary>
        /// <param name="nameWithPattern">The string containing potentially "menu-option-", "menu-start-", "menu-whatever"... prefixes.</param>
        /// <returns>The original name without the prefixes. If the input string is null or empty, the original string is returned.</returns>
        private static string GetOriginalName(string nameWithPattern)
        {
            if (string.IsNullOrEmpty(nameWithPattern) || !nameWithPattern.Contains('-')) {
                return nameWithPattern;
            }

            // Regular expression to match and remove the first two words separated by dashes
            var regex = new Regex(@"^[^-]+-[^-]+-");
            return regex.Replace(nameWithPattern, string.Empty);
        }
    }
}
