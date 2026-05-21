using JUS.Tool.Utils;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Containers.Converters;

/// <summary>
/// Converter between binary format and ALAR container. It supports versions
/// 2, and 3. It does also support compressed files (DCMP format).
/// </summary>
public class Binary2Alar : IConverter<IBinary, NodeContainerFormat>
{
    /// <inheritdoc />
    public NodeContainerFormat Convert(IBinary source)
    {
        bool isCompressed = CompressionUtils.IsCompressed(source.Stream);
        if (isCompressed) {
            source = new LzssDecompression().Convert(source);
        }

        byte version = Identifier.GetAlarVersion(source);
        NodeContainerFormat container = version switch {
            2 => new Binary2Alar2().Convert(source),
            3 => new Binary2Alar3().Convert(source),
            _ => throw new NotSupportedException($"Unsupported ALAR version: {version}")
        };

        container.Root.Tags["jus.alar.is_compressed"] = isCompressed;
        return container;
    }
}
