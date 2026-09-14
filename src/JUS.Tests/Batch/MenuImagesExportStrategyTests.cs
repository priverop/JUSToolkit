using JUS.Tool.BatchConverters.Strategies;
using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileSystem;
using Yarhl.TestFramework.Verify.Converters;

namespace JUS.Tests.Batch;

[TestFixture]
public class MenuImagesExportStrategyTests() : BaseExportStrategyTests(true)
{
    private static IEnumerable<TestCaseData> MenuAsset {
        get {
            if (!File.Exists(TestDataBase.SoftwareNitroRomPath)) {
                return [];
            }

            NitroRom rom = TestDataBase.ReadSoftware();
            IEnumerable<Node> nodes = [
                rom.Data.Children["Commu"].Children["commu_pack.aar"],
                rom.Data.Children["database"].Children["database.aar"],
                // rom.Data.Children["deckcheck"].Children["deckcheck.aar"],
                rom.Data.Children["input"].Children["input.aar"],
                rom.Data.Children["JArena"].Children["JArena.aar"],
                rom.Data.Children["jgalaxy"].Children["jgalaxy.aar"],
                rom.Data.Children["jpower"].Children["jpower.aar"],
                rom.Data.Children["option"].Children["option.aar"],
                rom.Data.Children["ruleselect"].Children["ruleselect.aar"],
                // rom.Data.Children["stageselect"].Children["stageselect.aar"],
                rom.Data.Children["topmenu"].Children["topmenu.aar"],
            ];
            return nodes.Select(n => new TestCaseData(n).SetArgDisplayNames(n.Path));
        }
    }

    protected override IAssetExportStrategy CreateStrategy() => new MenuImagesExportStrategy();

    [TestCaseSource(nameof(MenuAsset))]
    public void ExportedContainerAreNotDisposed(Node asset)
    {
        base.AssertExportedContainerAreNotDisposed(asset);
    }

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
            .AddExtraSettings(settings => {
                settings.Converters.Add(new NodeVerifyJsonConverter());
                settings.Converters.Add(new BinaryFormatVerifyJsonConverter());
            });
    }
}
