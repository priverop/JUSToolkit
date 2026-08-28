using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool;

public static class NodeExtensions
{
    extension(Node node)
    {
        public void ReplaceBinaryChildren(Node inputRoot)
        {
            foreach (Node inputChild in Navigator.IterateNodes(inputRoot)) {
                if (inputChild.IsContainer) {
                    continue;
                }

                string relativePath = Path.GetRelativePath(inputRoot.Path, inputChild.Path);
                Node? containerChild = Navigator.SearchNode(node, relativePath);
                if (containerChild is null) {
                    Console.WriteLine($"❌ {relativePath} node not found in the container");
                    continue;
                }

                // Create a new binary format, so if the input node is disposed, it won't affect ours.
                // This is a soft-clone, as it doesn't create a copy in memory.
                Console.WriteLine($"Replacing: {relativePath}");
                containerChild.ChangeFormat(new BinaryFormat(new DataStream(inputChild.Stream!)));
            }
        }
    }
}
