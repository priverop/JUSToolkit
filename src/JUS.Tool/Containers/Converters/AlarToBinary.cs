using JUS.Tool.Utils;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Containers.Converters;

/// <summary>
/// Converter between an ALAR container and binary format. It supports versions
/// 2, and 3. It does also support re-compression (DCMP format) via file tag.
/// </summary>
public class AlarToBinary : IConverter<NodeContainerFormat, BinaryFormat>
{
    /// <inheritdoc />
    public BinaryFormat Convert(NodeContainerFormat source)
    {
        ArgumentNullException.ThrowIfNull(source);

        AlarInfo info = source.Root.Children[Alar.InfoNodeName]?.GetFormatAs<AlarInfo>()
            ?? throw new FormatException("Missing info node");

        BinaryFormat actual = info switch {
            { Version: 2 } => new Alar2ToBinary().Convert(source),
            { Version: 3 } => new Alar3ToBinary().Convert(source),
            _ => throw new NotSupportedException("Unsupported format"),
        };

        bool isCompressed = source.Root.Tags.TryGetValue(Alar.CompressionTag, out object? isCompressedTag) && isCompressedTag is true;
        if (isCompressed) {
            actual = new LzssCompression().Convert(actual);
        }

        return actual;
    }
}
