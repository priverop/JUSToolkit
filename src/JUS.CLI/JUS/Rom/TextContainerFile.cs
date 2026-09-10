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
using JUS.Tool.Containers;
using JUS.Tool.Containers.Converters;
using JUS.Tool.Utils;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using static System.Net.WebRequestMethods;

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

                ProcessContainer(gameNode, alarPath, containerGroup);
            }
        }

        private static void ProcessContainer(Node gameNode, string alarPath, IEnumerable<Node> filesToInsert)
        {
            Node containerNode = Navigator.GetNode(gameNode, $"/root/data{alarPath}");
            Console.WriteLine($"Inserting text files in: /root/data{alarPath}.");

            Alar alar = containerNode.TransformWith<Binary2Alar3>().GetFormatAs<Alar>();
            foreach (Node fileToInsert in filesToInsert) {
                fileToInsert.Name = GetFileName(fileToInsert.Name);
                alar.InsertModification(fileToInsert);
            }
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
