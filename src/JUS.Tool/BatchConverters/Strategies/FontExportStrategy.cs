using JUS.Tool.Fonts;
using JUS.Tool.Utils;
using Microsoft.Extensions.Logging;
using Texim.Fonts;
using Texim.Formats.ImageSharp.Images;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.BatchConverters.Strategies;

/// <summary>
/// Stategy implement for creating fonts in exportable / editable formats: PNG + YAML.
/// </summary>
/// <remarks>
/// It exports every file under "/data/font", including: DSFont.aft, js8font.aft, jskfont.aft, jskfont_q.aft.
/// </remarks>
public class FontExportStrategy : IAssetExportStrategy
{
    private readonly ILogger<FontExportStrategy> logger = JusLoggerFactory.Instance.CreateLogger<FontExportStrategy>();

    /// <inheritdoc />
    public bool CanExport(Node asset) => asset.Parent?.Path.EndsWith("/data/font") ?? false;

    /// <inheritdoc />
    public IEnumerable<Node> Export(Node asset)
    {
        logger.LogDebug("Exporting font {FontName}", asset.Name);
        AlFont font = new Binary2AlFont().Convert(asset.GetFormatAs<IBinary>());

        BinaryTextFormat yaml = font.ConvertWith(new Font2Yaml());
        yield return new Node(asset.NameWithoutExtension + ".yml", yaml);

        BinaryFormat png = font.ConvertWith(new BitmapFont2RgbImage(font.Palettes.Palettes[0], BitmapFont2RgbImage.DefaultBorderColor))
            .ConvertWith(new RgbImage2BinaryPng());
        yield return new Node(asset.NameWithoutExtension + ".png", png);
    }
}
