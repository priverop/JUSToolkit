using JUS.CLI.JUS.Rom;
using NUnit.Framework;

namespace JUS.Tests.Cli.Rom;

[TestFixture]
public class TextPatternFileTests : BaseContainerStrategyTests<TextPatternFile>
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
    public override void AssertImportRawModifyExpectedNode(string inputPath, string expectedAssetPath)
    {
        base.AssertImportRawModifyExpectedNode(inputPath, expectedAssetPath);
    }

    private static IEnumerable<TestCaseData> GetMatchTestsFilenames() => [
        new("InfoDeck_bin/bin-deck-bb.bin"),
        new("deck_bin/jard_p/deck-jard-p000.bin"),
    ];

    private static IEnumerable<TestCaseData> GetImportedFilenames() => [
        new("InfoDeck_bin/bin-deck-bb.bin", "data/bin/InfoDeck.aar/bin/deck/bb.bin"),
        new("deck_bin/jadv/deck-jadv-000.bin", "data/deck/Deck.aar/deck/jadv/000.bin"),
        new("deck_bin/jard_p/deck-jard-p000.bin", "data/deck/Deck.aar/deck/jard/p000.bin"),
    ];
}
