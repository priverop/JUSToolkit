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
using System.Text.RegularExpressions;
using JUS.Tool.Containers;
using JUS.Tool.Containers.Converters;
using JUS.Tool.Utils;
using Yarhl.FileSystem;

namespace JUS.CLI.JUS.Rom
{
    /// <summary>
    /// Strategy Pattern: Interface for rom importing logic.
    /// </summary>
    public class TextPatternFile : IFileImportStrategy
    {
        private static readonly List<(Regex Pattern, string)> PatternList =
        [
            (new Regex(@"^bin-.*-.*\.bin$"), "/bin/InfoDeck.aar"), // "{container}/bin/deck/{file.Name}"
            (new Regex(@"^deck-.*-.*\.bin$"), "/deck/Deck.aar"), // "{container}/bin/deck/{file.Name}"
        ];

        public bool Matches(string filename)
        {
            return PatternList.Any(x => x.Pattern.IsMatch(filename));
        }

        /// <summary>
        /// Import files into the Rom.
        /// </summary>
        /// <param name="gameNode">The node of the Rom.</param>
        /// <param name="file">The input file to import.</param>
        public void Import(Node gameNode, Node file)
        {
            // Si no se encuentra, intenta encontrar la ruta interna usando patrones
            foreach ((Regex pattern, string containerPath) in PatternList) {
                if (pattern.IsMatch(file.Name)) {
                    string parent = GetParentName(file.Name);
                    file.Name = StringFunctions.GetOriginalName(file.Name);
                    ProcessContainer(gameNode, file, containerPath, parent);
                    return;
                }
            }

            Console.WriteLine($"File not compatible as text container: {file.Name}");
        }

        private static void ProcessContainer(Node gameNode, Node file, string containerPath, string parent)
        {
            Node containerNode = Navigator.SearchNode(gameNode, $"/root/data{containerPath}") ?? throw new FormatException($"Container not found /root/data{containerPath}");

            Alar alar = containerNode.TransformWith<Binary2Alar3>().GetFormatAs<Alar>()!;
            alar.InsertModification(file, parent);
            _ = containerNode.TransformWith(new Alar3ToBinary());

            Console.WriteLine($"File replaced: /root/data{containerPath}/{parent}/{file.Name}");
        }

        /// <summary>
        /// Gets the directory name of the file (parent). "bin-deck-bb.bin" will return "deck".
        /// </summary>
        /// <param name="name">The string containing potentially "bin-deck-", "bin-info-", "deck-play"... prefixes.</param>
        /// <returns>The directory name. If the input string is null or empty, the original string is returned.</returns>
        private static string GetParentName(string name)
        {
            // Regular expression to capture the second word
            var regex = new Regex(@"^[^-]+-([^-]+)-");
            Match match = regex.Match(name);

            return match.Groups[1].Value;
        }
    }
}
