using JUS.CLI.JUS.Rom;
using NUnit.Framework;

namespace JUS.Tests.Cli.Rom;

public class DemoImageFileTests : BaseContainerStrategyTests<DemoImageFile>
{
    [TestCaseSource(nameof(GetImportedFilenames))]
    public void AssertMatchesReturnsExcepted(string[] inputPaths)
    {
        foreach (string inputPath in inputPaths) {
            base.AssertMatchesReturnsExcepted(inputPath);
        }
    }

    [TestCaseSource(nameof(GetImportedFilenames))]
    public void AssertInputDoesNotMatchOtherStrategies(string[] inputPaths)
    {
        foreach (string inputPath in inputPaths) {
            base.AssertInputDoesNotMatchOtherStrategies(inputPath);
        }
    }

    [TestCaseSource(nameof(GetImportedFilenames))]
    public override void AssertImportPngDoesNotThrow(string[] inputPaths)
    {
        base.AssertImportPngDoesNotThrow(inputPaths);
    }

    private static IEnumerable<TestCaseData<string[]>> GetImportedFilenames() => [
        new(["images/comics/demo-bb_02.png"]),
        new(["images/comics/demo-bb_03.png", "images/comics/demo-bb_m_00.png", "images/comics/demo-bb_n_00.png"]),
        new(["images/comics/demo-bb_m_00.png"]),
        new(["images/comics/demo-dg_title.png"]),
        new(["images/comics/demo-opening_01.png"]),
    ];
}
