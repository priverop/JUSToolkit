using JUS.Tool.BatchConverters.Strategies;
using Yarhl.FileSystem;

namespace JUS.Tests.Batch;

[TestFixture]
public class JGalaxyTextExportStrategyTests() : BaseExportStrategyTests(includeHash: false)
{
    private static Lazy<Node> JGalaxyAsset => new(() => TestDataBase.ReadSoftware().Data.Children["jgalaxy"].Children["jgalaxy.aar"]);

    protected override IAssetExportStrategy CreateStrategy() => new JGalaxyTextExportStrategy(createTemplate: false);

    [Test]
    public void ExportedContainerAreNotDisposed()
    {
        base.AssertExportedContainerAreNotDisposed(JGalaxyAsset.Value);
    }

    [Test]
    public void StrategyMatches()
    {
        base.AssertStrategyMatches(JGalaxyAsset.Value);
    }

    [Test]
    public Task VerifyExportedContainer()
    {
        return base.AssertVerifyExportedContainer(JGalaxyAsset.Value);
    }
}
