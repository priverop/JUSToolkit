using JUS.Tool.BatchConverters.Strategies;
using Yarhl.FileSystem;
using Yarhl.TestFramework.Verify.Converters;

namespace JUS.Tests.Batch;

[TestFixture]
public class MenuImagesExportStrategyTests() : BaseExportStrategyTests(true)
{
    private static Lazy<Node> NitroRom { get; } = new(() => TestDataBase.ReadSoftware().Data);

    private static IEnumerable<TestCaseData> MenuAsset {
        get {
            if (!File.Exists(TestDataBase.SoftwareNitroRomPath)) {
                return [];
            }

            Node data = NitroRom.Value;
            IEnumerable<Node> nodes = [
                data.Children["Commu"].Children["commu_pack.aar"],
                data.Children["database"].Children["database.aar"],
                // data.Children["deckcheck"].Children["deckcheck.aar"],
                data.Children["input"].Children["input.aar"],
                data.Children["JArena"].Children["JArena.aar"],
                data.Children["jgalaxy"].Children["jgalaxy.aar"],
                data.Children["jpower"].Children["jpower.aar"],
                data.Children["option"].Children["option.aar"],
                data.Children["ruleselect"].Children["ruleselect.aar"],
                // data.Children["stageselect"].Children["stageselect.aar"],
                data.Children["topmenu"].Children["topmenu.aar"],
            ];
            return nodes.Select(n => new TestCaseData(n).SetArgDisplayNames(n.Path));
        }
    }

    protected override IAssetExportStrategy CreateStrategy() => new MenuImagesExportStrategy();

    [TestCaseSource(nameof(MenuAsset))]
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
        foreach (Node asset in MenuAsset.Select(t => (t.Arguments[0] as Node)!)) {
            Node output = new("out");
            output.Add(strategy.Export(asset));
            output.GetFormatAs<NodeContainerFormat>().MoveChildrenTo(root, true);
        }

        return Verifier.Verify(root)
            .UseDirectory(TestDataBase.VerifyTextsPath)
            .OnVerify(before: () => { }, after: root.Dispose)
            .AddExtraSettings(settings => {
                settings.Converters.Add(new NodeVerifyJsonConverter());
                settings.Converters.Add(new BinaryFormatVerifyJsonConverter());
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
