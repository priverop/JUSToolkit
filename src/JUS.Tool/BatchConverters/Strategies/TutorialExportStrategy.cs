using System.Text.RegularExpressions;
using JUS.Tool.Texts.Converters;
using JUS.Tool.Texts.Formats;
using JUS.Tool.Utils;
using Microsoft.Extensions.Logging;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUS.Tool.BatchConverters.Strategies;

/// <summary>
/// Strategy to export tutorials.
/// </summary>
/// <remarks>This strategy applies to: deckmake/tutorial and data/battle/tutorial*.bin.</remarks>
/// <param name="createTemplate">Value indicating whether to create PO templates or PO.</param>
public class TutorialExportStrategy(bool createTemplate) : IAssetExportStrategy
{
    private readonly ILogger<TutorialExportStrategy> logger = JusLoggerFactory.Instance.CreateLogger<TutorialExportStrategy>();
    private readonly string extension = createTemplate ? ".pot" : ".po";

    /// <inheritdoc />
    public AssetFormatKind ExportFormat => AssetFormatKind.Text;

    /// <inheritdoc />
    public bool CanExport(Node asset) => asset.Path.EndsWith("/data/deckmake/tutorial.bin")
        || Regex.IsMatch(asset.Path, "/data/battle/tutorial\\d.bin");

    /// <inheritdoc />
    public IEnumerable<Node> Export(Node asset)
    {
        logger.LogDebug("Reading tutorial: {Name}", asset.Name);
        Node exportedTutorials = new("tutorial");

        BinaryTextFormat binaryPo = asset.GetFormatAs<IBinary>()
            .ConvertWith<IBinary, Tutorial>(new Binary2Tutorial())
            .ConvertWith<Tutorial, Po>(new Tutorial2Po())
            .ConvertWith(new Po2Binary());
        exportedTutorials.Add(new Node(asset.Name + extension, binaryPo));

        return [exportedTutorials];
    }
}
