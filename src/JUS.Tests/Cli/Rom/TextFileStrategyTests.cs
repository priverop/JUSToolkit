using JUS.CLI.JUS.Rom;
using NUnit.Framework;
using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace JUS.Tests.Cli.Rom;

[TestFixture]
public class TextFileStrategyTests
{
    private static IEnumerable<TestCaseData> GetImportedFilenames() => [
        new("tutorial.bin", "data/deckmake/tutorial.bin"),
        new("tutorial5.bin", "data/battle/tutorial5.bin"),
        new("ability_t.bin", "data/bin/ability_t.bin"),
        new("title.bin", "data/bin/title.bin")
    ];

    [TestCaseSource(nameof(GetImportedFilenames))]
    public void Matches(string filename, string _)
    {
        var strategy = new TextFile();

        bool matches = strategy.Matches(filename);

        Assert.That(matches, Is.True);
    }

    [TestCaseSource(nameof(GetImportedFilenames))]
    public void Import(string filename, string expectedReplacedPath)
    {
        using var input = new Node(filename, new MyNewFormat());
        using NitroRom software = TestDataBase.ReadSoftware();

        var strategy = new TextFile();

        strategy.Import(software.Root, [input]);

        Node imported = Navigator.GetNode(software.Root, expectedReplacedPath);
        Assert.That(imported.Format, Is.InstanceOf(typeof(MyNewFormat)));
    }

    private sealed class MyNewFormat : IFormat;
}
