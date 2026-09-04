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
using JUS.Tool.Containers.Converters;
using JUS.Tool.Utils;
using Yarhl.FileSystem;
using Yarhl.IO;

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

        /// <inheritdoc/>
        public bool Matches(string filename)
        {
            return PatternList.Any(x => x.Pattern.IsMatch(filename));
        }

        /// <inheritdoc/>
        public void Import(Node gameNode, List<Node> files)
        {
            var filesGroupedByContainer = files.GroupBy(
                x => PatternList.FirstOrDefault(p => p.Pattern.IsMatch(x.Name)).Item2
            );

            foreach (var containerGroup in filesGroupedByContainer) {
                string alarPath = containerGroup.Key;

                ProcessContainer(gameNode, alarPath, containerGroup.ToArray());
            }
        }

        private static void ProcessContainer(Node gameNode, string alarPath, Node[] filesToInsert)
        {
            Node containerNode = Navigator.SearchNode(gameNode, $"/root/data{alarPath}");
            Console.WriteLine($"Inserting text with patterns in: /root/data{alarPath}");

            containerNode.TransformWith<Binary2Alar3>();
            foreach (Node fileToInsert in filesToInsert) {
                string parent = GetParentName(fileToInsert.Name);
                string filename = StringFunctions.GetOriginalName(fileToInsert.Name);

                // Soft-clone so if we dispose the input, the stream still exists.
                containerNode.Children[parent]
                    .Children[filename]
                    .ChangeFormat(new BinaryFormat(new DataStream(fileToInsert.Stream)));
            }

            _ = containerNode.TransformWith(new Alar3ToBinary());
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
