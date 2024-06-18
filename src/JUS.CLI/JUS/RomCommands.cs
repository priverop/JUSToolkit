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
using System.Text.RegularExpressions;
using JUSToolkit.CLI.JUS.Rom;
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
        private static readonly Dictionary<string, IFileImportStrategy> ImportStrategies = new()
        {
            { "tutorial.bin", new TextFile() },
            { "tutorial0.bin", new TextFile() },
            { "tutorial1.bin", new TextFile() },
            { "tutorial2.bin", new TextFile() },
            { "tutorial3.bin", new TextFile() },
            { "tutorial4.bin", new TextFile() },
            { "tutorial5.bin", new TextFile() },
            { "ability_t.bin", new TextFile() },
            { "bgm.bin", new TextFile() },
            { "chr_b_t.bin", new TextFile() },
            { "chr_s_t.bin", new TextFile() },
            { "clearlst.bin", new TextFile() },
            { "demo.bin", new TextFile() },
            { "infoname.bin", new TextFile() },
            { "location.bin", new TextFile() },
            { "piece.bin", new TextFile() },
            { "pname.bin", new TextFile() },
            { "rulemess.bin", new TextFile() },
            { "stage.bin", new TextFile() },
            { "title.bin", new TextFile() },
            { "jgalaxy.bin", new ContainerFile() },
            { "mission.bin", new ContainerFile() },
            { "battle.bin", new ContainerFile() },
        };

        private static readonly List<(Regex pattern, IFileImportStrategy strategy)> PatternStrategies = new()
        {
            (new Regex(@"^bin-.*-.*\.bin$"), new ContainerFile()),
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

            foreach (Node file in inputFiles.Children) {
                // Fixed names
                if (ImportStrategies.TryGetValue(file.Name, out IFileImportStrategy strategy)) {
                    strategy.Import(gameNode, file);
                } else {
                    // Pattern names for InfoDeck
                    bool matched = false;
                    foreach ((Regex pattern, IFileImportStrategy patternStrategy) in PatternStrategies) {
                        if (pattern.IsMatch(file.Name)) {
                            patternStrategy.Import(gameNode, file);
                            matched = true;
                            break;
                        }
                    }
                    if (!matched) {
                        Console.WriteLine($"File not compatible: {file.Name}");
                    }
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
