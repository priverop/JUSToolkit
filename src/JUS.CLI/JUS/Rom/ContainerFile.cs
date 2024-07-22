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
        private static readonly Dictionary<string, string> ContainerLocations = new() {
            { "jgalaxy.bin", "/jgalaxy/jgalaxy.aar" }, // Dónde está el .aar en el juego, pero faltaría la ruta interna del fichero .bin "{container}/jgalaxy/{file.Name}"
            { "mission.bin", "/jgalaxy/jgalaxy.aar" },
            { "battle.bin", "/jgalaxy/jgalaxy.aar" },
            { "jquiz.bin", "/jquiz/jquiz_pack.aar" },
        };

        private static readonly List<(Regex, string)> PatternList = new()
        {
            (new Regex(@"^bin-.*-.*\.bin$"), "/bin/InfoDeck.aar"), // "{container}/bin/deck/{file.Name}"
            (new Regex(@"^deck-.*-.*\.bin$"), "/deck/Deck.aar"), // "{container}/bin/deck/{file.Name}"
        };

        /// <summary>
        /// Import files into the Rom.
        /// </summary>
        /// <param name="gameNode">The node of the Rom.</param>
        /// <param name="file">The input file to import.</param>
        public void Import(Node gameNode, Node file)
        {
            if (ContainerLocations.TryGetValue(file.Name, out string path)) {
                ProcessContainer(gameNode, file, path);
                if (file.Name == "jquiz.bin") {
                    ModifyJQuizFont(gameNode);
                }
            } else {
                // Si no se encuentra, intenta encontrar la ruta interna usando patrones
                foreach ((Regex pattern, string containerPath) in PatternList) {
                    if (pattern.IsMatch(file.Name)) {
                        string parent = GetParentName(file.Name);
                        file.Name = GetOriginalName(file.Name);
                        ProcessContainer(gameNode, file, containerPath, parent);
                        return;
                    }
                }

                Console.WriteLine($"File not compatible as container: {file.Name}");
            }
        }

        private static void ProcessContainer(Node gameNode, Node file, string containerPath, string parent = null)
        {
            Node containerNode = Navigator.SearchNode(gameNode, $"/root/data{containerPath}")
                                .TransformWith<LzssDecompression>();

            Version alarVersion = Identifier.GetAlarVersion(containerNode.Stream);

            var newBinary = new BinaryFormat();

            // ToDo: We need to encapsulate/improve this
            if (alarVersion.Major == 3) {
                Alar3 alar = containerNode.TransformWith<Binary2Alar3>()
                .GetFormatAs<Alar3>();
                alar.InsertModification(file, parent);
                newBinary = alar.ConvertWith(new Alar3ToBinary());
            } else if (alarVersion.Major == 2) {
                Alar2 alar = containerNode.TransformWith<Binary2Alar2>()
                .GetFormatAs<Alar2>();
                alar.InsertModification(file); // ToDo: parent
                newBinary = alar.ConvertWith(new Alar2ToBinary());
            }

            containerNode.ChangeFormat(newBinary);

            Console.WriteLine($"File replaced: /root/data{containerPath}/{parent}/{file.Name}");
        }

        /// <summary>
        /// Copies the regular font to the jquiz font.
        /// </summary>
        /// <param name="gameNode">The rom.</param>
        private static void ModifyJQuizFont(Node gameNode)
        {
            Node regularFont = Navigator.SearchNode(gameNode, "/root/data/font/jskfont.aft");
            Node jquizFont = Navigator.SearchNode(gameNode, "/root/data/font/jskfont_q.aft");

            jquizFont.ChangeFormat(regularFont.Format!);
        }

        /// <summary>
        /// Remove the first two words separated by dashes "x-y-".
        /// </summary>
        /// <param name="nameWithPattern">The string containing potentially "bin-deck-", "bin-info-", "deck-play"... prefixes.</param>
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

        /// <summary>
        /// Gets the directory name of the file (parent). "bin-deck-bb.bin" will return "deck".
        /// </summary>
        /// <param name="name">The string containing potentially "bin-deck-", "bin-info-", "deck-play"... prefixes.</param>
        /// <returns>The directory name. If the input string is null or empty, the original string is returned.</returns>
        private static string GetParentName(string name)
        {
            if (string.IsNullOrEmpty(name) || !name.Contains('-')) {
                return null;
            }

            // Regular expression to capture the second word
            var regex = new Regex(@"^[^-]+-([^-]+)-");
            Match match = regex.Match(name);

            if (match.Success && match.Groups.Count > 1) {
                return match.Groups[1].Value;
            }

            return null;
        }
    }
}
