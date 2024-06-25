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
using System.Text.RegularExpressions;
using JUSToolkit.Containers;
using JUSToolkit.Containers.Converters;
using JUSToolkit.Graphics.Converters;
using JUSToolkit.Utils;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.CLI.JUS.Rom
{
    /// <summary>
    /// Strategy Pattern: Interface for rom importing logic.
    /// </summary>
    public class ContainerFile : IFileImportStrategy
    {
        private static readonly Dictionary<string, string[]> ContainerLocations = new() {
            { "jgalaxy.bin", ["/jgalaxy/jgalaxy.aar", "/jgalaxy/"] }, // Dónde está el .aar en el juego, pero faltaría la ruta interna del fichero .bin "{container}/jgalaxy/{file.Name}"
            { "mission.bin", ["/jgalaxy/jgalaxy.aar", "/jgalaxy/"] },
            { "battle.bin", ["/jgalaxy/jgalaxy.aar", "/jgalaxy/"] },
        };

        private static readonly List<(Regex, string[])> PatternList = new()
        {
            (new Regex(@"^bin-.*-.*\.bin$"), ["/bin/InfoDeck.aar", "/bin/deck/"]), // "{container}/bin/deck/{file.Name}"
        };

        /// <summary>
        /// Import files into the Rom.
        /// </summary>
        /// <param name="gameNode">The node of the Rom.</param>
        /// <param name="file">The input file to import.</param>
        public void Import(Node gameNode, Node file)
        {
            if (ContainerLocations.TryGetValue(file.Name, out string[] paths)) {
                ProcessContainer(gameNode, file, paths);
            } else {
                // Si no se encuentra, intenta encontrar la ruta interna usando patrones
                foreach ((Regex pattern, string[] paths2) in PatternList) {
                    if (pattern.IsMatch(file.Name)) {
                        file.Name = GetOriginalName(file.Name);
                        ProcessContainer(gameNode, file, paths2);
                        return;
                    }
                }
                Console.WriteLine($"File not compatible as container: {file.Name}");
            }
        }

        private static void ProcessContainer(Node gameNode, Node file, string[] paths)
        {
            Node containerNode = Navigator.SearchNode(gameNode, $"/root/data{paths[0]}")
                                .TransformWith<LzssDecompression>();

            Version alarVersion = Identifier.GetAlarVersion(containerNode.Stream);

            var newBinary = new BinaryFormat();

            // ToDo: We need to encapsulate/improve this
            if (alarVersion.Major == 3) {
                Alar3 alar = containerNode.TransformWith<Binary2Alar3>()
                .GetFormatAs<Alar3>();
                alar.InsertModification(file);
                newBinary = alar.ConvertWith(new Alar3ToBinary());
            } else if (alarVersion.Major == 2) {
                Alar2 alar = containerNode.TransformWith<Binary2Alar2>()
                .GetFormatAs<Alar2>();
                alar.InsertModification(file);
                newBinary = alar.ConvertWith(new Alar2ToBinary());
            }

            containerNode.ChangeFormat(newBinary);

            Console.WriteLine($"File replaced: /root/data{paths[0]}{paths[1]}{file.Name}");
        }

        /// <summary>
        /// Removes the substrings "bin-deck-" and "bin-info-" from the provided string in a case-insensitive manner.
        /// </summary>
        /// <param name="nameWithPattern">The string containing potentially "bin-deck-" or "bin-info-" prefixes.</param>
        /// <returns>The original name without the prefixes "bin-deck-" or "bin-info-". If the input string is null or empty, the original string is returned.</returns>
        private static string GetOriginalName(string nameWithPattern)
        {
            if (string.IsNullOrEmpty(nameWithPattern)) {
                return nameWithPattern;
            }

            nameWithPattern = nameWithPattern.Replace("bin-deck-", string.Empty, StringComparison.OrdinalIgnoreCase);
            nameWithPattern = nameWithPattern.Replace("bin-info-", string.Empty, StringComparison.OrdinalIgnoreCase);

            return nameWithPattern;
        }

    }
}
