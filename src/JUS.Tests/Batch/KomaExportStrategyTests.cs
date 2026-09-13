using JUS.Tool.BatchConverters.Strategies;
using Yarhl.FileSystem;

namespace JUS.Tests.Batch;

[TestFixture]
public class KomaExportStrategyTests() : BaseExportStrategyTests(true)
{
    private static Lazy<Node> KomaContainerAsset => new(
        () => TestDataBase.ReadSoftware().Data.Children["koma"].Children["koma.aar"]);

    protected override IAssetExportStrategy CreateStrategy() => new KomaExportStrategy();

    [Test]
    public void ExportedContainerAreNotDisposed()
    {
        base.AssertExportedContainerAreNotDisposed(KomaContainerAsset.Value);
    }

    [Test]
    public void StrategyMatches()
    {
        base.AssertStrategyMatches(KomaContainerAsset.Value);
    }

    [Test]
    public Task VerifyExportedContainer()
    {
        return base.AssertVerifyExportedContainer(KomaContainerAsset.Value);
    }
}
