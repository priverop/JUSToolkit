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
        private static readonly Dictionary<string, string> FileLocations = new() {
            { "tutorial.bin", "/deckmake/tutorial.bin" },
            { "tutorial0.bin", "/battle/tutorial0.bin" },
            { "tutorial1.bin", "/battle/tutorial1.bin" },
            { "tutorial2.bin", "/battle/tutorial2.bin" },
            { "tutorial3.bin", "/battle/tutorial3.bin" },
            { "tutorial4.bin", "/battle/tutorial4.bin" },
            { "tutorial5.bin", "/battle/tutorial5.bin" },
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
                if (FileLocations.TryGetValue(file.Name, out string value)) {
                    Node toReplace = Navigator.SearchNode(gameNode, $"/root/data{value}");
                    toReplace.ChangeFormat(file.Format!);
                    Console.WriteLine($"File replaced: /root/data{value}");
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
