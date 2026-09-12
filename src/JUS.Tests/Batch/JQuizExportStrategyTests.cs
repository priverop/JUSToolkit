using JUS.Tool.BatchConverters.Strategies;
using Yarhl.FileSystem;

namespace JUS.Tests.Batch;

[TestFixture]
public class JQuizExportStrategyTests() : BaseExportStrategyTests(includeHash: false)
{
    private static Lazy<Node> JQuizAsset => new(() => TestDataBase.ReadSoftware().Data.Children["jquiz"].Children["jquiz_pack.aar"]);

    protected override IAssetExportStrategy CreateStrategy() => new JQuizExportStrategy();

    [Test]
    public void ExportedContainerAreNotDisposed()
    {
        base.AssertExportedContainerAreNotDisposed(JQuizAsset.Value);
    }

    [Test]
    public void StrategyMatches()
    {
        base.AssertStrategyMatches(JQuizAsset.Value);
    }

    [Test]
    public Task VerifyExportedContainer()
    {
        return base.AssertVerifyExportedContainer(JQuizAsset.Value);
    }
}
