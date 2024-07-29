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
using Yarhl.FileSystem;

namespace JUSToolkit.CLI.JUS.Rom
{
    /// <summary>
    /// Strategy Pattern: Interface for rom importing logic.
    /// </summary>
    public class TextFile : IFileImportStrategy
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
            { "commwin.bin", "/bin/commwin.bin" },
            { "demo.bin", "/bin/demo.bin" },
            { "infoname.bin", "/bin/infoname.bin" },
            { "komatxt.bin", "/bin/komatxt.bin" },
            { "location.bin", "/bin/location.bin" },
            { "piece.bin", "/bin/piece.bin" },
            { "pname.bin", "/bin/pname.bin" },
            { "rulemess.bin", "/bin/rulemess.bin" },
            { "stage.bin", "/bin/stage.bin" },
            { "title.bin", "/bin/title.bin" },
        };

        /// <summary>
        /// Import files into the Rom.
        /// </summary>
        /// <param name="gameNode">The node of the Rom.</param>
        /// <param name="file">The input file to import.</param>
        public void Import(Node gameNode, Node file)
        {
            if (TextLocations.TryGetValue(file.Name, out string value)) {
                Node toReplace = Navigator.SearchNode(gameNode, $"/root/data{value}");
                toReplace.ChangeFormat(file.Format!);
                Console.WriteLine($"File replaced: /root/data{value}");
            }
        }
    }
}
