using JUS.Tool.Utils;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUS.Tool.Containers.Converters;

/// <summary>
/// Converter between binary format and ALAR container. It supports versions
/// 2, and 3. It does also support compressed files (DCMP format).
/// </summary>
public class Binary2Alar : IConverter<IBinary, Alar>
{
    /// <inheritdoc />
    public Alar Convert(IBinary source)
    {
        ArgumentNullException.ThrowIfNull(source);

        bool isCompressed = CompressionUtils.IsCompressed(source.Stream);
        if (isCompressed) {
            source = new LzssDecompression().Convert(source);
        }

        byte version = Identifier.GetAlarVersion(source);
        Alar container = version switch {
            2 => new Binary2Alar2().Convert(source),
            3 => new Binary2Alar3().Convert(source),
            _ => throw new NotSupportedException($"Unsupported ALAR version: {version}")
        };

        container.Root.Tags[Alar.CompressionTag] = isCompressed;
        return container;
    }
}
