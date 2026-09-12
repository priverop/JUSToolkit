using JUS.Tool.Utils;
using Microsoft.Extensions.Logging;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUS.Tool.BatchConverters.Strategies;

/// <summary>
/// Strategy to export uncategorized files.
/// </summary>
/// <param name="createTemplate">Value indicating whether to create PO templates or PO.</param>
/// <remarks>
/// This strategy export the following assets:
/// </remarks>
public class LooseTextsExportStrategy(bool createTemplate) : IAssetExportStrategy
{
    private readonly ILogger<LooseTextsExportStrategy> logger = JusLoggerFactory.Instance.CreateLogger<LooseTextsExportStrategy>();
    private readonly string extension = createTemplate ? ".pot" : ".po";

    /// <inheritdoc />
    public AssetFormatKind ExportFormat => AssetFormatKind.Text;

    /// <inheritdoc />
    public bool CanExport(Node asset)
    {
        if (!asset.Parent?.Path.EndsWith("/data/bin") ?? false) {
            return false;
        }

        return TextIdentifier.HasText(asset.Name);
    }

    /// <inheritdoc />
    public IEnumerable<Node> Export(Node asset)
    {
        logger.LogDebug("Reading loose file {Filename}", asset.Name);
        Type[] converterTypes = TextIdentifier.GetTextFormat(asset.Name);

        // Read: binary -> model
        object model = ConvertFormat.With(converterTypes[0], asset.GetFormatAs<IBinary>());

        // Export: model -> PO
        var po = (Po)ConvertFormat.With(converterTypes[1], model);
        BinaryTextFormat binaryPo = new Po2Binary().Convert(po);

        Node exported = new("unicos");
        exported.Add(new Node(asset.Name + extension, binaryPo));
        return [exported];
    }
}
