using JUS.Tool.BatchConverters.Strategies;
using Yarhl.FileSystem;

namespace JUS.Tests.Batch;

[TestFixture]
public class InfoDeckExportStrategyTests() : BaseExportStrategyTests(includeHash: false)
{
    private static Lazy<Node> DeckAsset => new(() => TestDataBase.ReadSoftware().Data.Children["bin"].Children["InfoDeck.aar"]);

    protected override IAssetExportStrategy CreateStrategy() => new InfoDeckExportStrategy(createTemplate: false);

    [Test]
    public void ExportedContainerAreNotDisposed()
    {
        base.AssertExportedContainerAreNotDisposed(DeckAsset.Value);
    }

    [Test]
    public void StrategyMatches()
    {
        base.AssertStrategyMatches(DeckAsset.Value);
    }

    [Test]
    public Task VerifyExportedContainer()
    {
        return base.AssertVerifyExportedContainer(DeckAsset.Value);
    }
}
