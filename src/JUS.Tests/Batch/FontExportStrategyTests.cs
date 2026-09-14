using JUS.Tool.BatchConverters.Strategies;
using Yarhl.FileSystem;
using Yarhl.TestFramework.Verify.Converters;

namespace JUS.Tests.Batch;

[TestFixture]
public class FontExportStrategyTests() : BaseExportStrategyTests(includeHash: true)
{
    private static IEnumerable<TestCaseData> FontAssets =>
        TestDataBase.ReadAndGetChildren("data/font")
            .Select(n => new TestCaseData(n).SetArgDisplayNames(n.Path));

    protected override IAssetExportStrategy CreateStrategy() => new FontExportStrategy();

    [TestCaseSource(nameof(FontAssets))]
    public void ExportedContainerAreNotDisposed(Node asset)
    {
        base.AssertExportedContainerAreNotDisposed(asset);
    }

    [TestCaseSource(nameof(FontAssets))]
    public void StrategyMatches(Node asset)
    {
        base.AssertStrategyMatches(asset);
    }

    [Test]
    public Task VerifyExportedContainer()
    {
        TestDataBase.IgnoreIfFileDoesNotExist(TestDataBase.SoftwareNitroRomPath);

        // To simplify the verification we export all at once
        IAssetExportStrategy strategy = CreateStrategy();
        var exports = new Node("root");
        foreach (Node asset in FontAssets.Select(t => (t.Arguments[0] as Node)!)) {
            exports.Add(strategy.Export(asset));
        }

        return Verifier.Verify(exports)
            .UseDirectory(TestDataBase.VerifyTextsPath)
            .AddExtraSettings(settings => {
                settings.Converters.Add(new NodeVerifyJsonConverter());
                settings.Converters.Add(new BinaryFormatVerifyJsonConverter());
            });
    }
}
