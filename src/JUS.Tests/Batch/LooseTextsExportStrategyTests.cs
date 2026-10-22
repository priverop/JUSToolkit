using JUS.Tool.BatchConverters.Strategies;
using JUS.Tool.Utils;
using Yarhl.FileSystem;
using Yarhl.TestFramework.Verify.Converters;

namespace JUS.Tests.Batch;

[TestFixture]
public class LooseTextsExportStrategyTests() : BaseExportStrategyTests(includeHash: false)
{
    private static Lazy<Node> NitroRom { get; } = new(() => TestDataBase.ReadSoftware().Data);

    private static IEnumerable<TestCaseData> LooseTextAssets {
        get {
            if (!File.Exists(TestDataBase.SoftwareNitroRomPath)) {
                return [];
            }

            return NitroRom.Value.Children["bin"].Children
                .Where(n => TextIdentifier.HasText(n.Name))
                .Select(n => new TestCaseData(n).SetArgDisplayNames(n.Path));
        }
    }

    protected override IAssetExportStrategy CreateStrategy() => new LooseTextsExportStrategy(false);

    [TestCaseSource(nameof(LooseTextAssets))]
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
        var root = new Node("root");
        foreach (Node asset in LooseTextAssets.Select(t => (t.Arguments[0] as Node)!)) {
            Node output = new("out");
            output.Add(strategy.Export(asset));
            output.GetFormatAs<NodeContainerFormat>().MoveChildrenTo(root, true);

        }

        return Verifier.Verify(root)
            .UseDirectory(TestDataBase.VerifyTextsPath)
            .OnVerify(before: () => { }, after: root.Dispose)
            .AddExtraSettings(settings => {
                settings.Converters.Add(new NodeVerifyJsonConverter(serializeFormat: false));
            });
    }

    [OneTimeTearDown]
    public void DisposeAssets()
    {
        if (NitroRom.IsValueCreated) {
            NitroRom.Value.Dispose();
        }
    }
}
