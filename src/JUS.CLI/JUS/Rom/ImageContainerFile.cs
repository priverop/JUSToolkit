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
using System.Linq;
using System.Text.RegularExpressions;
using JUSToolkit.BatchConverters;
using JUSToolkit.Containers;
using JUSToolkit.Containers.Converters;
using JUSToolkit.Utils;
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
        private static readonly Dictionary<string, string[]> ContainerLocations = new() {
            { "menu-Commu-code.png", ["code.dig", "code.atm", "/Commu/commu_pack.aar"] },
            { "menu-Commu-matchmake.png", ["matchmake.dig", "matchmake.atm", "/Commu/commu_pack.aar"] },
            { "menu-Commu-member.png", ["member.dig", "member.atm", "/Commu/commu_pack.aar"] },
            { "menu-Commu-member_data.png", ["member.dig", "member_data.atm", "/Commu/commu_pack.aar"] },
            { "menu-Commu-member_wireless.png", ["member.dig", "member_wireless.atm", "/Commu/commu_pack.aar"] },
            { "menu-Commu-sure02.png", ["sure02.dig", "sure02.atm", "/Commu/commu_pack.aar"] },
            { "menu-Commu-top_left.png", ["top_left.dig", "top_left.atm", "/Commu/commu_pack.aar"] },
            { "menu-Commu-top_right.png", ["top_right.dig", "top_right.atm", "/Commu/commu_pack.aar"] },
            { "menu-JArena-a_result01.png", ["a_result01.dig", "a_result01.atm", "/JArena/JArena.aar"] },
            { "menu-JArena-congra_top00.png", ["congra_top00.dig", "congra_top00.atm", "/JArena/JArena.aar"] },
            { "menu-JArena-mis01.png", ["mis01.dig", "mis01.atm", "/JArena/JArena.aar"] },
            { "menu-JArena-mis_top10.png", ["mis_top10.dig", "mis_top10.atm", "/JArena/JArena.aar"] },
            { "menu-JArena-ranking_list.png", ["ranking_list.dig", "ranking_list.atm", "/JArena/JArena.aar"] },
            { "menu-database-list_bg0.png", ["list_bg0.dig", "list_bg0.atm", "/database/database.aar"] },
            { "menu-database-personal_bg.png", ["personal_bg.dig", "personal_bg.atm", "/database/database.aar"] },
            { "menu-database-personal_bg2.png", ["personal_bg2.dig", "personal_bg2.atm", "/database/database.aar"] },
            { "menu-database-playing.png", ["playing.dig", "playing.atm", "/database/database.aar"] },
            { "menu-database-story_bg0.png", ["story_bg0.dig", "story_bg0.atm", "/database/database.aar"] },
            { "menu-database-story_bg1.png", ["story_bg1.dig", "story_bg1.atm", "/database/database.aar"] },
            { "menu-deckcheck-win_a.png", ["win.dig", "win_a.atm", "/deckcheck/deckcheck.aar"] },
            { "menu-deckcheck-win_g.png", ["win.dig", "win_g.atm", "/deckcheck/deckcheck.aar"] },
            { "menu-deckselect-deck_standby00.png", ["deck_standby00.dig", "deck_standby00.atm", "/deckselect/deckselect.aar"] },
            { "menu-deckselect-deck_standby01.png", ["deck_standby01.dig", "deck_standby01.atm", "/deckselect/deckselect.aar"] },
            { "menu-input-mode_bg0.png", ["mode_bg0.dig", "mode_bg0.atm", "/input/input.aar"] },
            { "menu-input-mode_bg1.png", ["mode_bg1.dig", "mode_bg1.atm", "/input/input.aar"] },
            { "menu-input-mode_bg2.png", ["mode_bg2.dig", "mode_bg2.atm", "/input/input.aar"] },
            { "menu-input-mode_bg3.png", ["mode_bg3.dig", "mode_bg3.atm", "/input/input.aar"] },
            { "menu-jgalaxy-ba_m_sel01.png", ["ba_m_sel01.dig", "ba_m_sel01.atm", "/jgalaxy/jgalaxy.aar"] },
            { "menu-jgalaxy-victory00.png", ["victory00.dig", "victory00.atm", "/jgalaxy/jgalaxy.aar"] },
            { "menu-jgalaxy-victory01.png", ["victory00.dig", "victory01.atm", "/jgalaxy/jgalaxy.aar"] },
            { "menu-jgalaxy-win_sel.png", ["win_sel.dig", "win_sel.atm", "/jgalaxy/jgalaxy.aar"] },
            { "menu-jpower-jp.png", ["jp.dig", "jp.atm", "/jpower/jpower.aar"] },
            { "menu-jpower-jp_top.png", ["jp_top.dig", "jp_top.atm", "/jpower/jpower.aar"] },
            { "menu-option-info00.png", ["info00.dig", "info00.atm", "/option/option.aar"] },
            { "menu-option-option.png", ["option.dig", "option.atm", "/option/option.aar"] },
            { "menu-ruleselect-rule_sel00.png", ["rule_sel00.dig", "rule_sel00.atm", "/ruleselect/ruleselect.aar"] },
            { "menu-stageselect-st_sel01.png", ["st_sel01.dig", "st_sel01.atm", "/stageselect/stageselect.aar"] },
            { "menu-topmenu-top_bg01.png", ["top_bg01.dig", "top_bg01.atm", "/topmenu/topmenu.aar"] },
        };

        private static readonly Dictionary<char, char> SpecialDigNumbers = new() {
            { '0', '3' },
            { '1', '5' },
            { '2', '7' },
            { '3', '9' },
        };

        private static readonly List<(Regex, string[])> PatternList = new()
        {
            (new Regex(@"^demo-.*\.png$"), ["/demo/demo.aar"]),
        };

        /// <summary>
        /// Import files into the Rom.
        /// </summary>
        /// <param name="gameNode">The node of the Rom.</param>
        /// <param name="file">The input file to import.</param>
        public void Import(Node gameNode, Node file)
        {
            if (ContainerLocations.TryGetValue(file.Name, out string[] imageInfo)) {
                file.Name = StringFunctions.GetOriginalName(file.Name);
                ProcessContainer(gameNode, file, imageInfo);
            } else {
                // Si no se encuentra, intenta encontrar la ruta interna usando patrones
                foreach ((Regex pattern, string[] containerPath) in PatternList) {
                    if (pattern.IsMatch(file.Name)) {
                        file.Name = StringFunctions.GetDemoName(file.Name);

                        // Ignore the _n_ and _m_, because we obtain them later with the original
                        if (!IsSpecialDemoNM(file.Name)) {
                            // Get the Dig, Atm and Alar
                            string[] demoInfo = GetDemoInfo(file.Name, containerPath);
                            ProcessContainer(gameNode, file, demoInfo, true);
                        }

                        return;
                    }
                }

                Console.WriteLine($"File not compatible as image container: {file.Name}");
            }
        }

        private static void ProcessContainer(Node gameNode, Node pngFile, string[] imageInfo, bool transparentTile = false)
        {
            // 1 - Search the Original Alar3
            Node originalAlar = Navigator.SearchNode(gameNode, $"/root/data{imageInfo[2]}") ?? throw new FormatException($"Container not found /root/data{imageInfo[2]}");
            _ = originalAlar.TransformWith<Binary2Alar3>();

            // 2 - Insert the Png into the Alar3
            IConverter image2Alar3;

            // Special demo needs 3 atm, 3 pngs and 1 dig (imageInfo[0])
            if (IsSpecialDemo(pngFile.Name) && !pngFile.Name.Contains("opening") && !pngFile.Name.Contains("sel01")) {
                string[] atms = GetSpecialAtms(imageInfo[1]);
                Node[] pngs = GetSpecialPngs(pngFile);
                image2Alar3 = new Demo2Alar3(pngs, imageInfo[0], atms, transparentTile);
            } else {
                image2Alar3 = new Png2Alar3(pngFile, imageInfo[0], imageInfo[1], transparentTile);
            }

            Alar3 newAlar = originalAlar
                .TransformWith(image2Alar3)
                .GetFormatAs<Alar3>();

            BinaryFormat newBinary = newAlar.ConvertWith(new Alar3ToBinary());

            // 3 - Sustituirlo
            _ = originalAlar.ChangeFormat(newBinary);

            Console.WriteLine($"File replaced: /root/data{imageInfo[2]}/{pngFile.Name}");
        }

        /// <summary>
        /// Get the atm and dig names.
        /// </summary>
        /// <param name="pngName">The name of the Png we are importing.</param>
        /// <param name="containerPath">The path of the alar container of the file.</param>
        /// <returns>An array with the DIG, ATM, and ContainerPath (.aar) names.</returns>
        private static string[] GetDemoInfo(string pngName, string[] containerPath)
            => [GetDemoDigName(pngName) + ".dig",
                Path.GetFileNameWithoutExtension(pngName) + ".atm",
                containerPath[0]];

        /// <summary>
        /// Returns true if the png is a Special Demo Image.
        /// </summary>
        /// <param name="pngName">The name of the Png we are importing.</param>
        /// <returns>True if it's a special demo image (03, 05, 07, 09, _m_ or _n_). False otherwise.</returns>
        private static bool IsSpecialDemo(string pngName) =>
            pngName.Contains("_03") ||
            pngName.Contains("_05") ||
            pngName.Contains("_07") ||
            pngName.Contains("_09") ||
            pngName.Contains("_m_") ||
            pngName.Contains("_n_");

        /// <summary>
        /// Returns true if the png is a _m_ or _n_ Special Demo Image.
        /// </summary>
        /// <param name="pngName">The name of the Png we are importing.</param>
        /// <returns>True if it's a special demo image (_m_ or _n_). False otherwise.</returns>
        private static bool IsSpecialDemoNM(string pngName) =>
            pngName.Contains("_m_") ||
            pngName.Contains("_n_");

        /// <summary>
        /// Obtains the name of the corresponding Dig.
        /// </summary>
        /// <remarks>
        /// bb_00.png => bb_00.dig
        /// bb_m_00.png => bb_03.dig
        /// bb_n_00.png => bb_03.dig
        /// bb_m_01.png => bb_05.dig
        /// bb_n_01.png => bb_05.dig
        /// bb_m_02.png => bb_07.dig
        /// bb_n_02.png => bb_07.dig
        /// bb_m_03.png => bb_09.dig
        /// bb_n_03.png => bb_09.dig.
        /// </remarks>
        private static string GetDemoDigName(string pngName)
        {
            // SpecialChar is the 4th character of the demo png name.
            // jj_m_00.png => m
            // jj_n_00.png => n
            // jj_03.png => not n nor m, so it's not a special one
            if (pngName.Contains("_m_") || pngName.Contains("_n_")) {
                // jj_m_01.png => 1
                char number = pngName[6];
                string manga = pngName[..2];
                return manga + "_0" + SpecialDigNumbers.GetValueOrDefault(number);
            } else {
                return Path.GetFileNameWithoutExtension(pngName);
            }
        }

        /// <summary>
        /// Constructs the names of the _m_ and _n_ files for the given base node name and extension.
        /// </summary>
        /// <param name="nodeName">The base name of the node (e.g., "bb_03.png").</param>
        /// <param name="extension">The file extension (e.g., ".png", ".atm").</param>
        /// <returns>An array of strings containing the names of the base, _m_, and _n_ files.</returns>
        private static string[] GetSpecialFileNames(string nodeName, string extension)
        {
            string manga = nodeName[..2]; // "bb"
            char number = nodeName[4]; // '3'

            // Use the dictionary to find the corresponding index for _m_ and _n_
            if (!SpecialDigNumbers.Any(kv => kv.Value == number)) {
                throw new InvalidOperationException($"Value {number} is not found in SpecialDigNumbers.");
            }

            char specialNumber = SpecialDigNumbers.First(kv => kv.Value == number).Key;

            // Construct the names of the _m_ and _n_ files
            string nameOfMFile = $"{manga}_m_0{specialNumber}{extension}";
            string nameOfNFile = $"{manga}_n_0{specialNumber}{extension}";

            return new[] { nodeName, nameOfMFile, nameOfNFile };
        }

        /// <summary>
        /// Returns an array of Nodes with the given PNG Node, the _m_ and _n_ Nodes.
        /// </summary>
        /// <remarks>
        /// bb_03.png => [bb_03.png, bb_m_00.png, bb_n_00.png]
        /// bb_05.png => [bb_05.png, bb_m_01.png, bb_n_01.png]
        /// bb_07.png => [bb_07.png, bb_m_02.png, bb_n_02.png]
        /// bb_09.png => [bb_09.png, bb_m_03.png, bb_n_03.png].
        /// </remarks>
        private static Node[] GetSpecialPngs(Node png)
        {
            string[] fileNames = GetSpecialFileNames(png.Name, ".png");

            // Retrieve the corresponding Nodes with the original name
            Node mNode = png.Parent.Children["demo-" + fileNames[1]] ??
                    throw new FormatException("Special m file not found: " + fileNames[1]);
            Node nNode = png.Parent.Children["demo-" + fileNames[2]] ??
                    throw new FormatException("Special nfile not found: " + fileNames[2]);

            // Return the array of Nodes
            return new[] { png, mNode, nNode };
        }

        /// <summary>
        /// Returns an array of strings with the names of the given ATM file, the _m_, and the _n_ files.
        /// </summary>
        /// <remarks>
        /// bb_03.atm => [bb_03.atm, bb_m_00.atm, bb_n_00.atm]
        /// bb_05.atm => [bb_05.atm, bb_m_01.atm, bb_n_01.atm]
        /// bb_07.atm => [bb_07.atm, bb_m_02.atm, bb_n_02.atm]
        /// bb_09.atm => [bb_09.atm, bb_m_03.atm, bb_n_03.atm].
        /// </remarks>
        private static string[] GetSpecialAtms(string atm)
        {
            return GetSpecialFileNames(atm, ".atm");
        }
    }
}
