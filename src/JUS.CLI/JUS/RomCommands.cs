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
using JUS.CLI.JUS.Rom;
using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.CLI.JUS
{
    /// <summary>
    /// Commands to generate the Rom.
    /// </summary>
    public static class RomCommands
    {
        private static readonly IFileImportStrategy[] Strategies =
        [
            new TextFile(),
            new TextContainerFile(),
            new TextPatternFile(),
            new MenuImageFile(),
            new DemoImageFile(),
            new SpriteDtx3ImageFile(),
        ];

        /// <summary>
        /// Import files into the Rom.
        /// </summary>
        /// <param name="game">The path to the Rom.</param>
        /// <param name="input">The path with the files to import.</param>
        /// <param name="output">The output directory.</param>
        public static void Import(string game, string input, string output)
        {
            Console.WriteLine($"Importing {input}");

            Node gameNode = NodeFactory.FromFile(game, "root", FileOpenMode.Read)
                .TransformWith<Binary2NitroRom>();

            Node inputFiles = NodeFactory.FromDirectory(input);
            inputFiles.SortChildren((x, y) => string.Compare(x.Name, y.Name, StringComparison.CurrentCulture));

            // Files with no strategies
            // In most strategies, we modify Node.Name, so we need to do this first
            var orphanFiles = inputFiles.Children.Where(file => !Strategies.Any(strategy => strategy.Matches(file.Name)));
            foreach (Node orphan in orphanFiles) {
                Console.WriteLine("These files won't be imported, as they don't match with any importer:");
                Console.WriteLine(orphan.Name);
            }

            foreach (IFileImportStrategy strategy in Strategies) {
                var matchedFiles = inputFiles.Children.Where(f => strategy.Matches(f.Name)).ToList();
                if (matchedFiles.Count > 0) {
                    strategy.Import(gameNode, matchedFiles);
                }
            }

            var nitroParameters = new NitroRom2BinaryParams { DecompressedProgram = true };
            gameNode.TransformWith(new NitroRom2Binary(nitroParameters));

            gameNode.Stream!.WriteTo(Path.Combine(output, "new_game.nds"));

            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Import a font into the Rom.
        /// </summary>
        /// <param name="game">The path to the Rom.</param>
        /// <param name="font">The path with the font to import.</param>
        /// <param name="output">The output directory.</param>
        public static void ImportFont(string game, string font, string output)
        {
            Console.WriteLine($"Importing {font}");

            Node gameNode = NodeFactory.FromFile(game, "root", FileOpenMode.Read)
                .TransformWith<Binary2NitroRom>();

            Node fontNode = NodeFactory.FromFile(font, FileOpenMode.Read);

            // Regular Font
            Node toReplace = Navigator.SearchNode(gameNode, "/root/data/font/jskfont.aft")!;
            toReplace.ChangeFormat(fontNode.Format!);
            Console.WriteLine("File replaced: /root/data/font/jskfont.aft");

            // JQuiz Font
            Node toReplace_q = Navigator.SearchNode(gameNode, "/root/data/font/jskfont_q.aft")!;
            toReplace_q.ChangeFormat(fontNode.Format!);
            Console.WriteLine("File replaced: /root/data/font/jskfont_q.aft");

            var nitroParameters = new NitroRom2BinaryParams { DecompressedProgram = true };
            gameNode.TransformWith(new NitroRom2Binary(nitroParameters));

            gameNode.Stream!.WriteTo(Path.Combine(output, "new_game_font.nds"));

            Console.WriteLine("Done!");
        }
    }
}
