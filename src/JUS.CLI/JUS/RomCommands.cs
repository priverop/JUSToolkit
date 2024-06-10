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
using JUSToolkit.Containers;
using JUSToolkit.Containers.Converters;
using JUSToolkit.Graphics.Converters;
using JUSToolkit.Utils;
using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.CLI.JUS
{
    /// <summary>
    /// Commands to generate the Rom.
    /// </summary>
    public static class RomCommands
    {
        // ToDo: Remove filename of the second string, we only need the directory
        private static readonly Dictionary<string, string> TextLocations = new() {
            { "tutorial.bin", "/deckmake/tutorial.bin" },
            { "tutorial0.bin", "/battle/tutorial0.bin" },
            { "tutorial1.bin", "/battle/tutorial1.bin" },
            { "tutorial2.bin", "/battle/tutorial2.bin" },
            { "tutorial3.bin", "/battle/tutorial3.bin" },
            { "tutorial4.bin", "/battle/tutorial4.bin" },
            { "tutorial5.bin", "/battle/tutorial5.bin" },
            { "ability_t.bin", "/bin/ability_t.bin" },
            { "bgm.bin", "/bin/bgm.bin" },
            { "chr_b_t.bin", "/bin/chr_b_t.bin" },
            { "chr_s_t.bin", "/bin/chr_s_t.bin" },
            { "clearlst.bin", "/bin/clearlst.bin" },
            { "demo.bin", "/bin/demo.bin" },
            { "infoname.bin", "/bin/infoname.bin" },
            { "location.bin", "/bin/location.bin" },
            { "piece.bin", "/bin/piece.bin" },
            { "pname.bin", "/bin/pname.bin" },
            { "rulemess.bin", "/bin/rulemess.bin" },
            { "stage.bin", "/bin/stage.bin" },
            { "title.bin", "/bin/title.bin" },
        };

        private static readonly Dictionary<string, string> ContainerLocations = new() {
            { "jgalaxy.bin", "/jgalaxy/jgalaxy.aar" },
            { "mission.bin", "/jgalaxy/jgalaxy.aar" },
            { "battle.bin", "/jgalaxy/jgalaxy.aar" },
        };

        /// <summary>
        /// Import files into the Rom.
        /// </summary>
        /// <param name="game">The path to the Rom.</param>
        /// <param name="input">The path with the files to import.</param>
        /// <param name="output">The output directory.</param>
        public static void Import(string game, string input, string output)
        {
            Node gameNode = NodeFactory.FromFile(game, "root", FileOpenMode.Read)
                .TransformWith<Binary2NitroRom>();

            Node inputFiles = NodeFactory.FromDirectory(input);
            int test = 0;

            foreach (Node file in inputFiles.Children) {
                if (TextLocations.TryGetValue(file.Name, out string value)) {
                    Node toReplace = Navigator.SearchNode(gameNode, $"/root/data{value}");
                    toReplace.ChangeFormat(file.Format!);
                    Console.WriteLine($"File replaced: /root/data{value}");
                } else if (ContainerLocations.TryGetValue(file.Name, out string container)) {
                    Node containerNode = Navigator.SearchNode(gameNode, $"/root/data{container}");

                    if (test == 0) {
                        containerNode.TransformWith<LzssDecompression>();

                        Version alarVersion = Identifier.GetAlarVersion(containerNode.Stream);

                        // ToDo: We need to encapsulate this
                        if (alarVersion.Major == 3) {
                            containerNode.TransformWith<Binary2Alar3>();
                        } else if (alarVersion.Major == 2) {
                            containerNode.TransformWith<Binary2Alar2>();
                        }
                    }

                    // Alar3File
                    Node toReplace = Navigator.SearchNode(containerNode, $"/root/data{container}/jgalaxy/{file.Name}");
                    Alar3File alarFileOld = toReplace.GetFormatAs<Alar3File>();
                    var newAlarFile = new Alar3File(file.Stream) {
                        FileID = alarFileOld.FileID,
                        Unknown = alarFileOld.Unknown,
                        Offset = alarFileOld.Offset,
                        Size = (uint)alarFileOld.Stream.Length,
                        Unknown2 = alarFileOld.Unknown2,
                        Unknown3 = alarFileOld.Unknown3,
                        Unknown4 = alarFileOld.Unknown4,
                    };

                    // Necesitamos un Alar3File nuevo con el stream de file. Para eso habrá que clonar el de toReplace?
                    toReplace.ChangeFormat(newAlarFile);
                    containerNode.TransformWith<Alar3ToBinary>();
                    test++;
                    Console.WriteLine($"File replaced: /root/data{container}/jgalaxy/{file.Name}");
                } else {
                    Console.WriteLine($"File not compatible: {file.Name}");
                }
            }

            gameNode.TransformWith<NitroRom2Binary>();
            gameNode.Stream.WriteTo(Path.Combine(output, "new_game.nds"));

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Import files into the Rom.
        /// </summary>
        /// <param name="game">The path to the Rom.</param>
        /// <param name="input">The path with the files to import.</param>
        /// <param name="output">The output directory.</param>
        public static void Export(string game, string input, string output)
        {
            Console.WriteLine("Not yet implemented.");
        }
    }
}
