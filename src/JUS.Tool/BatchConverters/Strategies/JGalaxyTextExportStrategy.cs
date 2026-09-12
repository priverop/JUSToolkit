using JUS.Tool.Containers.Converters;
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
/// Strategy to export text from jgalaxy.aar container.
/// </summary>
public class JGalaxyTextExportStrategy(bool createTemplate) : IAssetExportStrategy
{
    private readonly ILogger<JGalaxyTextExportStrategy> logger = JusLoggerFactory.Instance.CreateLogger<JGalaxyTextExportStrategy>();
    private readonly string extension = createTemplate ? ".pot" : ".po";

    /// <inheritdoc />
    public AssetFormatKind ExportFormat => AssetFormatKind.Text;

    /// <inheritdoc />
    public bool CanExport(Node asset) => asset.Path.EndsWith("data/jgalaxy/jgalaxy.aar");

    /// <inheritdoc />
    public IEnumerable<Node> Export(Node asset)
    {
        logger.LogDebug("Reading JGalaxy");
        Node exportedJGalaxy = new("jgalaxy");

        using NodeContainerFormat container = new Binary2Alar().Convert(asset.GetFormatAs<IBinary>());
        Node containerRoot = container.Root.Children["jgalaxy"];

        var jGalaxyPo = containerRoot.Children["jgalaxy.bin"].GetFormatAs<IBinary>()
            .ConvertWith<IBinary, JGalaxyComplex>(new Binary2JGalaxyComplex())
            .ConvertWith<JGalaxyComplex, Po>(new JGalaxyComplex2Po())
            .ConvertWith(new Po2Binary());
        exportedJGalaxy.Add(new Node($"jgalaxy-jgalaxy.bin{extension}", jGalaxyPo));

        var battlePo = containerRoot.Children["battle.bin"].GetFormatAs<IBinary>()
            .ConvertWith<IBinary, JGalaxySimple>(new Binary2JGalaxySimple())
            .ConvertWith<JGalaxySimple, Po>(new JGalaxySimple2Po())
            .ConvertWith(new Po2Binary());
        exportedJGalaxy.Add(new Node($"jgalaxy-battle.bin{extension}", battlePo));

        var missionPo = containerRoot.Children["mission.bin"].GetFormatAs<IBinary>()
            .ConvertWith<IBinary, JGalaxySimple>(new Binary2JGalaxySimple())
            .ConvertWith<JGalaxySimple, Po>(new JGalaxySimple2Po())
            .ConvertWith(new Po2Binary());
        exportedJGalaxy.Add(new Node($"jgalaxy-mission.bin{extension}", missionPo));

        return [exportedJGalaxy];
    }
}
