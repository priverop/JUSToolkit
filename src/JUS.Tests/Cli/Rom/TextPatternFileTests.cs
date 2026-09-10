using JUS.CLI.JUS.Rom;
using JUS.Tool.Containers.Converters;
using NUnit.Framework;
using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tests.Cli.Rom;

[TestFixture]
public class TextPatternFileTests
{
    private static IEnumerable<TestCaseData> GetImportedFilenames() => [
        new("InfoDeck_bin/bin-deck-bb.bin", "data/bin/InfoDeck.aar", "data/bin/InfoDeck.aar/bin/deck/bb.bin"),
        new("deck_bin/deck-jadv-000.bin", "data/deck/Deck.aar", "data/deck/Deck.aar/deck/jadv/000.bin"),
    ];

    [TestCaseSource(nameof(GetImportedFilenames))]
    public void Matches(string filePath, string _1, string _2)
    {
        using Node input = CreateInputNode(filePath);
        var strategy = new TextPatternFile();

        bool matches = strategy.Matches(input.Name);

        Assert.That(matches, Is.True);
    }

    [TestCaseSource(nameof(GetImportedFilenames))]
    public void Import(string filePath, string containerPath, string expectedReplacedPath)
    {
        using Node input = CreateInputNode(filePath);
        using NitroRom software = TestDataBase.ReadSoftware();

        var strategy = new TextPatternFile();

        strategy.Import(software.Root, [input]);

        Navigator.SearchNode(software.Root, containerPath).TransformWith(new Binary2Alar());
        Node imported = Navigator.SearchNode(software.Root, expectedReplacedPath);
        byte[] importedData = imported.Stream.ReadBytes(2);
        Assert.That(importedData, Is.EqualTo([0xCA, 0xFE]));
    }

    private static Node CreateInputNode(string filePath)
    {
        DataStream importedData = DataStreamFactory.FromArray([0xCA, 0xFE]);
        var input = new Node(Path.GetFileName(filePath), new BinaryFormat(importedData));

        var root = new Node("root");
        string? directoriesPath = Path.GetDirectoryName(filePath);
        if (directoriesPath is not null) {
            NodeFactory.CreateContainersForChild(root, directoriesPath, input);
        }

        return input;
    }
}
