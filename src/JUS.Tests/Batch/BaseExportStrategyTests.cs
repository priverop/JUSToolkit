using JUS.Tool.BatchConverters.Strategies;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.TestFramework.Verify.Converters;

namespace JUS.Tests.Batch;

public abstract class BaseExportStrategyTests(bool includeHash)
{
    protected abstract IAssetExportStrategy CreateStrategy();

    protected void AssertExportedContainerAreNotDisposed(Node asset)
    {
        TestDataBase.IgnoreIfFileDoesNotExist(TestDataBase.SoftwareNitroRomPath);

        IEnumerable<Node> exports = CreateStrategy().Export(asset);

        var root = new Node("root", new NodeContainerFormat(exports));
        foreach (Node exportedNode in Navigator.IterateNodes(root)) {
            Assert.That(exportedNode.Disposed, Is.False);

            if (exportedNode.Format is IBinary { Stream: DataStream dataStream }) {
                Assert.That(dataStream.Disposed, Is.False);
            }
        }
    }

    protected void AssertStrategyMatches(Node asset)
    {
        bool canExport = CreateStrategy().CanExport(asset);
        Assert.That(canExport, Is.True);
    }

    protected Task AssertVerifyExportedContainer(Node asset)
    {
        TestDataBase.IgnoreIfFileDoesNotExist(TestDataBase.SoftwareNitroRomPath);

        IEnumerable<Node> exports = CreateStrategy().Export(asset);

        return Verifier.Verify(exports)
            .UseDirectory(TestDataBase.VerifyTextsPath)
            .AddExtraSettings(settings => {
                settings.Converters.Add(new NodeVerifyJsonConverter(serializeFormat: includeHash));
                if (includeHash) {
                    settings.Converters.Add(new BinaryFormatVerifyJsonConverter());
                }
            });
    }
}
