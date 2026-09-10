using JUS.Tool.Fonts;
using JUS.Tool.Utils;
using Microsoft.Extensions.Logging;
using Texim.Fonts;
using Texim.Formats.ImageSharp.Images;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.BatchConverters;

/// <summary>
/// Converter that creates a new container with the game font assets ready to export in editable formats.
/// </summary>
/// <remarks>This container does not modify the input (it doesn't transform any of its nodes).</remarks>
public class JusFontAssetsExporter : IConverter<NodeContainerFormat, NodeContainerFormat>
{
    private readonly ILogger<JusFontAssetsExporter> logger = JusLoggerFactory.Instance.CreateLogger<JusFontAssetsExporter>();

    /// <inheritdoc />
    public NodeContainerFormat Convert(NodeContainerFormat source)
    {
        Node fontContainer = source.Root.Children["data"].Children["font"];

        IEnumerable<Node> exportDsFont = ExportFont(fontContainer.Children["DSFont.aft"]);
        IEnumerable<Node> exportjs8Font = ExportFont(fontContainer.Children["js8font.aft"]);
        IEnumerable<Node> exportjskFont = ExportFont(fontContainer.Children["jskfont.aft"]);
        IEnumerable<Node> exportjskQFont = ExportFont(fontContainer.Children["jskfont_q.aft"]);

        return new NodeContainerFormat([..exportDsFont, ..exportjs8Font, ..exportjskFont, ..exportjskQFont]);
    }

    private IEnumerable<Node> ExportFont(Node input)
    {
        logger.LogDebug("Exporting font {FontName}", input.Name);
        AlFont font = new Binary2AlFont().Convert(input.GetFormatAs<IBinary>());

        BinaryTextFormat yaml = font.ConvertWith(new Font2Yaml());
        yield return new Node(input.NameWithoutExtension + ".yml", yaml);

        BinaryFormat png = font.ConvertWith(new BitmapFont2RgbImage(font.Palettes.Palettes[0], BitmapFont2RgbImage.DefaultBorderColor))
            .ConvertWith(new RgbImage2BinaryPng());
        yield return new Node(input.NameWithoutExtension + ".png", png);
    }
}
