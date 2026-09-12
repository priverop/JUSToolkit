using JUS.Tool.Containers.Converters;
using JUS.Tool.Texts.Converters;
using JUS.Tool.Utils;
using Microsoft.Extensions.Logging;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.BatchConverters.Strategies;

/// <summary>
/// Strategy to export jquiz_pack.aar container.
/// </summary>
public class JQuizExportStrategy : IAssetExportStrategy
{
    private readonly ILogger<JQuizExportStrategy> logger = JusLoggerFactory.Instance.CreateLogger<JQuizExportStrategy>();

    /// <inheritdoc />
    public AssetFormatKind ExportFormat => AssetFormatKind.Text;

    /// <inheritdoc />
    public bool CanExport(Node asset) => asset.Path.EndsWith("/data/jquiz/jquiz_pack.aar");

    /// <inheritdoc />
    public IEnumerable<Node> Export(Node asset)
    {
        logger.LogDebug("Reading JQuiz");
        Node exportedJQuiz = new("jquiz");

        using NodeContainerFormat container = new Binary2Alar().Convert(asset.GetFormatAs<IBinary>());
        Node jquiz = container.Root.Children["jquiz"].Children["jquiz.bin"];

        jquiz.TransformWith(new Binary2JQuiz())
            .TransformWith(new JQuiz2Po());

        foreach (Node quiz in jquiz.Children) {
            // Soft-clone to have an independent node
            exportedJQuiz.Add(new Node(quiz.Name, new BinaryFormat(quiz.Stream.AsDataStream())));
        }

        return [exportedJQuiz];
    }
}
