// Copyright (c) 2026 Priverop

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
using Yarhl.FileSystem;

namespace JUS.CLI.JUS.Rom
{
    /// <summary>
    /// Imports .aar directly to the game.
    /// </summary>
    public class RawContainerFile : IFileImportStrategy
    {
        /// <inheritdoc/>
        public bool Matches(string filename)
        {
            return Path.GetExtension(filename) == ".aar";
        }

        /// <inheritdoc/>
        public void Import(Node gameNode, List<Node> files)
        {
            foreach (Node container in files) {
                Node toReplace = Navigator.IterateNodes(gameNode).FirstOrDefault(x => x.Name == container.Name) ?? throw new FormatException($"Container not found {container}");
                toReplace.ChangeFormat(container.Format);
                Console.WriteLine($"Container replaced: {container.Name}");
            }
        }
    }
}
