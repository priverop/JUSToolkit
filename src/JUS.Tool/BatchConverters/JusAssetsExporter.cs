using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace JUS.Tool.BatchConverters;

/// <summary>
/// Converter that creates a new container with the all the game assets ready to export for editing.
/// </summary>
/// <remarks>This container does not modify the input (it doesn't transform any of its nodes).</remarks>
/// <param name="languageCode">The target translation language, or null to generate templates.</param>
public class JusAssetsExporter(string? languageCode) : IConverter<NodeContainerFormat, NodeContainerFormat>
{
    /// <inheritdoc />
    public NodeContainerFormat Convert(NodeContainerFormat source)
    {
        ArgumentNullException.ThrowIfNull(source);

        bool createPoTemplates = string.IsNullOrEmpty(languageCode);
        NodeContainerFormat exportedTextNodes = new JusTextAssetsExporter(createPoTemplates).Convert(source);
        var texts = new Node(languageCode ?? "templates");
        texts.Add(exportedTextNodes.Root.Children);

        return new NodeContainerFormat([texts]);
    }
}
