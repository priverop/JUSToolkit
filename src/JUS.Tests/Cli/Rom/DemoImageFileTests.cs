using JUS.CLI.JUS.Rom;
using NUnit.Framework;

namespace JUS.Tests.Cli.Rom;

public class DemoImageFileTests : BaseContainerStrategyTests<DemoImageFile>
{
    [TestCaseSource(nameof(GetMatchTestsFilenames))]
    public override void AssertMatchesReturnsExcepted(string inputPath)
    {
        base.AssertMatchesReturnsExcepted(inputPath);
    }

    [TestCaseSource(nameof(GetMatchTestsFilenames))]
    public override void AssertInputDoesNotMatchOtherStrategies(string inputPath)
    {
        base.AssertInputDoesNotMatchOtherStrategies(inputPath);
    }

    [TestCaseSource(nameof(GetImportedFilenames))]
    public override void AssertImportModifyExpectedNode(string inputPath, string expectedAssetPath)
    {
        base.AssertImportModifyExpectedNode(inputPath, expectedAssetPath);
    }

    private static IEnumerable<TestCaseData> GetMatchTestsFilenames() => [
        new("InfoDeck_bin/demo-bb_m_00.bin"),
    ];

    private static IEnumerable<TestCaseData> GetImportedFilenames() => [
        new("InfoDeck_bin/demo-bb_m_00.bin", "data/bin/InfoDeck.aar/bin/deck/bb.bin"),
    ];
}
