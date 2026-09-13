using JUS.Tool.Containers.Converters;
using JUS.Tool.Graphics;
using JUS.Tool.Graphics.Converters;
using JUS.Tool.Utils;
using Microsoft.Extensions.Logging;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.BatchConverters.Strategies;

/// <summary>
/// Strategy to export the koma images from koma.aar.
/// </summary>
/// <remarks>This strategy requires and will access to ../bin/kshape.bin and ../bin/koma.bin.</remarks>
public class KomaExportStrategy : IAssetExportStrategy
{
    private readonly ILogger<KomaExportStrategy> logger = JusLoggerFactory.Instance.CreateLogger<KomaExportStrategy>();

    /// <inheritdoc />
    public AssetFormatKind ExportFormat => AssetFormatKind.Image;

    /// <inheritdoc />
    public bool CanExport(Node asset) => asset.Path.EndsWith("/data/koma/koma.aar");

    /// <inheritdoc />
    public IEnumerable<Node> Export(Node asset)
    {
        // Exporting koma images require two other files:
        Node koma = asset.Parent?.Parent?.Children["bin"].Children["koma.bin"]
            ?? throw new InvalidOperationException("Cannot get bin/koma.bin from relative path");
        Node kshape = asset.Parent?.Parent?.Children["bin"].Children["kshape.bin"]
            ?? throw new InvalidOperationException("Cannot get bin/kshape.bin from relative path");

        IEnumerable<Node> images = Export(
            asset.GetFormatAs<IBinary>(),
            koma.GetFormatAs<IBinary>(),
            kshape.GetFormatAs<IBinary>());

        var output = new Node("koma");
        output.Add(images);
        return [output];
    }

    /// <summary>
    /// Export the koma images from the binary inputs.
    /// </summary>
    /// <param name="binaryContainer">Binary format of koma.aar container.</param>
    /// <param name="binaryKoma">Binary format of koma.bin.</param>
    /// <param name="binaryKShape">Binary format of kshape.bin</param>
    /// <returns>Exported images classified by manga.</returns>
    public IEnumerable<Node> Export(IBinary binaryContainer, IBinary binaryKoma, IBinary binaryKShape)
    {
        using NodeContainerFormat container = new Binary2Alar().Convert(binaryContainer);
        Node containerRoot = container.Root.Children["koma"];

        Koma koma = new Binary2Koma().Convert(binaryKoma);
        KShapeSprites kshapes = new BinaryKShape2SpriteCollection().Convert(binaryKShape);

        Node output = new("root");
        foreach (KomaElement komaElement in koma) {
            string textureName = $"{komaElement.KomaName}.dtx";
            if (!containerRoot.Children.TryGet(textureName, out Node? texture)) {
                logger.LogError("Missing texture {Name}", textureName);
                continue;
            }

            var converter = new Dtx4ToBitmap(kshapes, komaElement);
            BinaryFormat png = converter.Convert(texture.GetFormatAs<IBinary>());

            string manga = komaElement.KomaName.Split('_')[0];
            NodeFactory.CreateContainersForChild(output, manga, new Node(komaElement.KomaName + ".png", png));
        }

        return output.Children;
    }
}
