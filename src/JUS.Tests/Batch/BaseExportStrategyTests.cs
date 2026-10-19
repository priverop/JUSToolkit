using JUS.Tool.BatchConverters.Strategies;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.TestFramework.Verify.Converters;

namespace JUS.Tests.Batch;

public abstract class BaseExportStrategyTests(bool includeHash)
{
    private IEnumerable<Node> exports = [];

    protected abstract IAssetExportStrategy CreateStrategy();

    protected void AssertStrategyMatches(Node asset)
    {
        bool canExport = CreateStrategy().CanExport(asset);
        Assert.That(canExport, Is.True);
    }

    protected Task AssertVerifyExportedContainer(Node asset)
    {
        TestDataBase.IgnoreIfFileDoesNotExist(TestDataBase.SoftwareNitroRomPath);

        exports = CreateStrategy().Export(asset).ToList();

        return Verifier.Verify(exports)
            .UseDirectory(TestDataBase.VerifyTextsPath)
            .AddExtraSettings(settings => {
                settings.Converters.Add(new NodeVerifyJsonConverter(serializeFormat: includeHash, maxChildren: 20));
                if (includeHash) {
                    settings.Converters.Add(new BinaryFormatVerifyJsonConverter());
                }
            });
    }

    [TearDown]
    public void DisposeExports()
    {
        foreach (Node node in exports) {
            node.Dispose();
        }

        exports = [];
    }
}
