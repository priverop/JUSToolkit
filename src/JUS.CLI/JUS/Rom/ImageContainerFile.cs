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
                        if (!IsSpecialDemoNM(file.Name)) {
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
            if (IsSpecialDemo(pngFile.Name)) {
                string[] atms = GetSpecialAtms(pngFile, imageInfo[1]);
                Node[] pngs = GetSpecialPngs(pngFile);
                // ¿Qué hacemos con el imageInfo?
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
        /// <returns>An array with the DIG, ATM, and ContainerPath names.</returns>
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
        /// bb_n_03.png => bb_09.dig
        /// </remarks>
        private static string GetDemoDigName(string pngName)
        {
            // SpecialChar is the 4th character of the demo png name.
            // jj_m_00.png => m
            // jj_n_00.png => n
            // jj_03.png => not n nor m, so it's not a special one
            char specialChar = pngName[3];
            if (specialChar is 'm' or 'n') {
                // jj_m_01.png => 1
                char number = pngName[6];
                string manga = pngName[..2];
                return manga + "_0" + SpecialDigNumbers.GetValueOrDefault(number);
            } else {
                return Path.GetFileNameWithoutExtension(pngName);
            }
        }

        /// <summary>
        /// Returns an array of Nodes with the given Node, the _m_ and the _n_ Nodes for the specified extension (png or atm).
        /// </summary>
        /// <param name="node">The original Node.</param>
        /// <param name="extension">The file extension (e.g., ".png", ".atm").</param>
        /// <returns>An array of Nodes.</returns>
        private static Node[] GetSpecialNodes(Node node, string extension)
        {
            string nodeName = node.Name; // Assume node.Name is "bb_03.png" or "bb_03.atm"
            string manga = nodeName[..2]; // "bb"
            char number = nodeName[3]; // '3'

            // Use the dictionary to find the corresponding index for _m_ and _n_
            if (!SpecialDigNumbers.TryGetValue(number, out char specialNumber)) {
                throw new InvalidOperationException($"Number {number} is not in SpecialDigNumbers.");
            }

            // Construct the names of the _m_ and _n_ files with the specified extension
            string nameOfMNode = $"{manga}_m_0{specialNumber}{extension}";
            string nameOfNNode = $"{manga}_n_0{specialNumber}{extension}";

            // Retrieve the corresponding Nodes
            Node mNode = node.Parent.Children[nameOfMNode];
            Node nNode = node.Parent.Children[nameOfNNode];

            // Return the array of Nodes
            return new[] { node, mNode, nNode };
        }

        /// <summary>
        /// Returns an array of Nodes for the given PNG Node.
        /// </summary>
        /// <remarks>
        /// bb_03.png => [bb_03.png, bb_m_00.png, bb_n_00.png]
        /// bb_05.png => [bb_05.png, bb_m_01.png, bb_n_01.png]
        /// bb_07.png => [bb_07.png, bb_m_02.png, bb_n_02.png]
        /// bb_09.png => [bb_09.png, bb_m_03.png, bb_n_03.png]
        /// </remarks>
        private static Node[] GetSpecialPngs(Node png)
        {
            return GetSpecialNodes(png, ".png");
        }

        /// <summary>
        /// Returns an array of Nodes for the given ATM Node.
        /// </summary>
        /// <remarks>
        /// bb_03.atm => [bb_03.atm, bb_m_00.atm, bb_n_00.atm]
        /// bb_05.atm => [bb_05.atm, bb_m_01.atm, bb_n_01.atm]
        /// bb_07.atm => [bb_07.atm, bb_m_02.atm, bb_n_02.atm]
        /// bb_09.atm => [bb_09.atm, bb_m_03.atm, bb_n_03.atm]
        /// </remarks>
        private static Node[] GetSpecialAtms(Node png, string atmName)
        {
            Node atm = png.Parent.Children[atmName];
            return GetSpecialNodes(atm, ".atm");
        }
    }
}
