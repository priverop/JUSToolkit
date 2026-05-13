using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileSystem;

namespace JUS.Tool.Discovery;

/// <summary>
/// Defines the supported software of this library.
/// </summary>
public static class SupportedSoftware
{
    /// <summary>
    /// Gets the game code for the supported version.
    /// </summary>
    public static string GameCode => "AJUJ";

    /// <summary>
    /// Gets a value indicating whether the node belongs to a compatible software version.
    /// </summary>
    /// <param name="assetNode">The node to test.</param>
    /// <param name="root">The node representing the root of the game from the input node.</param>
    /// <returns>Value indicating whether the node belongs to a compatible software, or null if it cannot be determined.</returns>
    public static bool? IsFromCompatibleSoftware(Node assetNode, out Node root)
    {
        ProgramInfo? info = GetProgramInfo(assetNode);
        root = assetNode;
        while (root.Parent is not null && info is null) {
            root = root.Parent;
            info = GetProgramInfo(root);
        }

        return info is null ? null : GameCode == info.GameCode;

        static ProgramInfo? GetProgramInfo(Node node) =>
            node.Children["system"]?.Children["info"]?.Format as ProgramInfo;
    }
}
