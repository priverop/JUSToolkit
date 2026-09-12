using JUS.Tool.BatchConverters.Strategies;
using Yarhl.FileSystem;

namespace JUS.Tests.Batch;

[TestFixture]
public class ComicImagesExportStrategyTests() : BaseExportStrategyTests(true)
{
    private static Lazy<Node> DemoAsset => new(() => TestDataBase.ReadSoftware().Data.Children["demo"].Children["demo.aar"]);

    protected override IAssetExportStrategy CreateStrategy() => new ComicImagesExportStrategy();

    [Test]
    public void ExportedContainerAreNotDisposed()
    {
        base.AssertExportedContainerAreNotDisposed(DemoAsset.Value);
    }

    [Test]
    public void StrategyMatches()
    {
        base.AssertStrategyMatches(DemoAsset.Value);
    }

    [Test]
    public Task VerifyExportedContainer()
    {
        return base.AssertVerifyExportedContainer(DemoAsset.Value);
    }
}
