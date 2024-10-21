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
using JUSToolkit.BatchConverters;
using JUSToolkit.Containers;
using JUSToolkit.Containers.Converters;
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
            { "menu-Commu-code.png", "/Commu/commu_pack.aar" },
            { "menu-Commu-sure02.png", "/Commu/commu_pack.aar" },
            { "menu-Commu-top_left.png", "/Commu/commu_pack.aar" },
            { "menu-Commu-top_right.png", "/Commu/commu_pack.aar" },
            { "menu-JArena-a_result01.png", "/JArena/JArena.aar" },
            { "menu-JArena-congra_top00.png", "/JArena/JArena.aar" },
            { "menu-JArena-mis01.png", "/JArena/JArena.aar" },
            { "menu-JArena-mis_top10.png", "/JArena/JArena.aar" },
            { "menu-JArena-ranking_list.png", "/JArena/JArena.aar" },
            { "menu-database-list_bg0.png", "/database/database.aar" },
            { "menu-database-personal_bg.png", "/database/database.aar" },
            { "menu-database-personal_bg2.png", "/database/database.aar" },
            { "menu-database-playing.png", "/database/database.aar" },
            { "menu-database-story_bg0.png", "/database/database.aar" },
            { "menu-database-story_bg1.png", "/database/database.aar" },
            { "menu-deckselect-deck_standby00.png", "/deckselect/deckselect.aar" },
            { "menu-deckselect-deck_standby01.png", "/deckselect/deckselect.aar" },
            { "menu-input-mode_bg0.png", "/input/input.aar" },
            { "menu-input-mode_bg1.png", "/input/input.aar" },
            { "menu-input-mode_bg2.png", "/input/input.aar" },
            { "menu-input-mode_bg3.png", "/input/input.aar" },
            { "menu-jgalaxy-ba_m_sel01.png", "/jgalaxy/jgalaxy.aar" },
            { "menu-jgalaxy-victory00.png", "/jgalaxy/jgalaxy.aar" },
            { "menu-jgalaxy-win_sel.png", "/jgalaxy/jgalaxy.aar" },
            { "menu-jpower-jp.png", "/jpower/jpower.aar" },
            { "menu-jpower-jp_top.png", "/jpower/jpower.aar" },
            { "menu-option-info00.png", "/option/option.aar" },
            { "menu-option-option.png", "/option/option.aar" },
            { "menu-ruleselect-rule_sel00.png", "/ruleselect/ruleselect.aar" },
            { "menu-stageselect-st_sel01.png", "/stageselect/stageselect.aar" },
            { "menu-topmenu-top_bg01.png", "/topmenu/topmenu.aar" },
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

                Console.WriteLine($"File not compatible as image container: {file.Name}");
            }
        }

        private static void ProcessContainer(Node gameNode, Node file, string containerPath)
        {
            // 1 - Buscar el Alar3 Original
            Node originalAlar = Navigator.SearchNode(gameNode, $"/root/data{containerPath}") ?? throw new FormatException($"Container not found /root/data{containerPath}");
            originalAlar.TransformWith<Binary2Alar3>();

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
