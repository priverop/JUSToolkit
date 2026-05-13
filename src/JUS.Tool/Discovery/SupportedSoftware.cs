using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileSystem;

namespace JUS.Tool.Discovery;

public static class SupportedSoftware
{
    public static string GameCode => "AJUJ";

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
