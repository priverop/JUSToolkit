using JUS.Tool.Utils;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool;

/// <summary>
/// Extensions for nodes.
/// </summary>
public static class NodeExtensions
{
    extension(Node node)
    {
        /// <summary>
        /// Replace the binary content of the current nodes matching the nodes from the input by path.
        /// </summary>
        /// <param name="inputRoot">The nodes with binary data to replace.</param>
        public void ReplaceBinaryChildren(Node inputRoot)
        {
            foreach (Node inputChild in Navigator.IterateNodes(inputRoot)) {
                if (inputChild.IsContainer) {
                    continue;
                }

                string relativePath = Path.GetRelativePath(inputRoot.Path, inputChild.Path);
                Node? containerChild = Navigator.SearchNodeOrDefault(node, relativePath);
                if (containerChild is null) {
                    Logger.DisplayError($"{relativePath} node not found in the container");
                    continue;
                }

                // Create a new binary format, so if the input node is disposed, it won't affect ours.
                // This is a soft-clone, as it doesn't create a copy in memory.
                Console.WriteLine($"Replacing: {relativePath}");
                containerChild.ChangeFormat(new BinaryFormat(new DataStream(inputChild.Stream)));
            }
        }
    }
}
