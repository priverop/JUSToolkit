using System.Text;

namespace JUS.Tool.Containers.Converters;

internal static class AlarPathHash
{
    public static ushort ComputeV1(string path)
    {
        string hashPath = GetHashPath(path);
        byte[] data = Encoding.ASCII.GetBytes(hashPath);

        uint hash = 0;
        foreach (byte ch in data) {
            uint tmp = (hash << 1) ^ ch;
            hash = (tmp & 0xFFFF) ^ (tmp >> 16);
        }

        return (ushort)hash;
    }

    public static ushort ComputeV2(string path)
    {
        string hashPath = GetHashPath(path);
        byte[] data = Encoding.ASCII.GetBytes(hashPath);

        int hash = 0;
        foreach (byte ch in data) {
            hash ^= (ushort)(hash << 1) | ch;
        }

        return (ushort)hash;
    }

    private static string GetHashPath(string containerPath)
    {
        int extensionIdx = containerPath.LastIndexOf('.');
        return extensionIdx == -1 ? containerPath : containerPath[..extensionIdx];
    }
}
