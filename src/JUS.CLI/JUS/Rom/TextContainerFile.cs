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

using JUS.Tool;
using JUS.Tool.Containers.Converters;
using Yarhl.FileSystem;

namespace JUS.CLI.JUS.Rom
{
    /// <summary>
    /// Strategy Pattern: Interface for rom importing logic.
    /// </summary>
    public class TextContainerFile : IFileImportStrategy
    {
        private static readonly Dictionary<string, string> ContainerLocations = new() {
            { "jgalaxy-jgalaxy.bin", "/jgalaxy/jgalaxy.aar" },
            { "jgalaxy-mission.bin", "/jgalaxy/jgalaxy.aar" },
            { "jgalaxy-battle.bin", "/jgalaxy/jgalaxy.aar" },
            { "jquiz.bin", "/jquiz/jquiz_pack.aar" },
        };

        /// <inheritdoc/>
        public bool Matches(string filename)
        {
            return ContainerLocations.ContainsKey(filename);
        }

        /// <inheritdoc/>
        public void Import(Node gameNode, List<Node> files)
        {
            var filesGroupedByContainer = files.GroupBy(x => ContainerLocations[x.Name]);

            foreach (var containerGroup in filesGroupedByContainer) {
                string alarPath = containerGroup.Key;

                ProcessContainer(gameNode, alarPath, containerGroup.ToArray());
            }
        }

        private static void ProcessContainer(Node gameNode, string alarPath, Node[] filesToInsert)
        {
            Node containerNode = Navigator.SearchNode(gameNode, $"/root/data{alarPath}") ?? throw new FormatException($"Container not found /root/data{alarPath}");
            Console.WriteLine($"Inserting text files in: /root/data{alarPath}.");

            containerNode.TransformWith<Binary2Alar3>();
            foreach (Node fileToInsert in filesToInsert) {
                fileToInsert.Name = GetFileName(fileToInsert.Name);
            }

            var inputRoot = new Node("root", new NodeContainerFormat());
            inputRoot.Add(filesToInsert);

            containerNode.ReplaceBinaryChildren(inputRoot);
            _ = containerNode.TransformWith(new Alar3ToBinary());

        }

        /// <summary>
        /// Gets the file name without the container prefix. "jgalaxy-mission.bin" returns "mission.bin".
        /// </summary>
        private static string GetFileName(string name)
        {
            if (string.IsNullOrEmpty(name) || !name.Contains('-')) {
                return name;
            }

            return name[(name.IndexOf('-') + 1)..];
        }
    }
}
