using JUS.Tool.BatchConverters.Strategies;
using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileSystem;
using Yarhl.TestFramework.Verify.Converters;

namespace JUS.Tests.Batch;

[TestFixture]
public class TutorialExportStrategyTests() : BaseExportStrategyTests(includeHash: false)
{
    private static IEnumerable<TestCaseData> TutorialAssets {
        get {
            if (!File.Exists(TestDataBase.SoftwareNitroRomPath)) {
                return [];
            }

            NitroRom rom = TestDataBase.ReadSoftware();
            IEnumerable<Node> nodes = [
                rom.Data.Children["deckmake"].Children["tutorial.bin"],
                ..rom.Data.Children["battle"].Children
                    .Where(n => n.Name.StartsWith("tutorial") && n.Extension == ".bin"),
            ];
            return nodes.Select(n => new TestCaseData(n).SetArgDisplayNames(n.Path));
        }
    }

    protected override IAssetExportStrategy CreateStrategy() => new TutorialExportStrategy(false);

    [TestCaseSource(nameof(TutorialAssets))]
    public void ExportedContainerAreNotDisposed(Node asset)
    {
        base.AssertExportedContainerAreNotDisposed(asset);
    }

    [TestCaseSource(nameof(TutorialAssets))]
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
        foreach (Node asset in TutorialAssets.Select(t => (t.Arguments[0] as Node)!)) {
            Node output = new("out");
            output.Add(strategy.Export(asset));
            output.GetFormatAs<NodeContainerFormat>().MoveChildrenTo(root, true);

        }

        return Verifier.Verify(root)
            .UseDirectory(TestDataBase.VerifyTextsPath)
            .AddExtraSettings(settings => {
                settings.Converters.Add(new NodeVerifyJsonConverter(serializeFormat: false));
            });
    }
}
