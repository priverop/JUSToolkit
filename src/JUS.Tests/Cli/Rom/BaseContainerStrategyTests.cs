using JUS.CLI.JUS;
using JUS.CLI.JUS.Rom;
using JUS.Tool.Containers.Converters;
using NUnit.Framework;
using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tests.Cli.Rom;

public abstract class BaseContainerStrategyTests<T>
    where T : IFileImportStrategy, new()
{
#pragma warning disable S2743
    private static readonly byte[] ImportedData = [0xCA, 0xFE];
#pragma warning restore S2743

    public virtual void AssertMatchesReturnsExcepted(string inputPath)
    {
        using Node input = CreateInputNode(inputPath);
        var strategy = new T();

        bool matches = strategy.Matches(input.Name);

        Assert.That(matches, Is.True);
    }

    public virtual void AssertInputDoesNotMatchOtherStrategies(string inputPath)
    {
        foreach (IFileImportStrategy strategy in RomCommands.Strategies) {
            if (strategy.GetType() == typeof(T)) {
                continue;
            }

            bool matches = strategy.Matches(inputPath);
            Assert.That(matches, Is.False, $"Unexpected match from: {strategy.GetType().FullName}");
        }
    }

    public virtual void AssertImportModifyExpectedNode(string inputPath, string expectedAssetPath)
    {
        using Node input = CreateInputNode(inputPath);
        using NitroRom software = TestDataBase.ReadSoftware();
        var strategy = new T();

        strategy.Import(software.Root, [input]);

        // Unpack all the containers to access to the imported file.
        Node imported = GetNodeUnpackingContainers(software.Root, expectedAssetPath);

        // Compare content on this node is from this test.
        byte[] importedData = imported.Stream.ReadBytes(2);
        Assert.That(importedData, Is.EqualTo(ImportedData));
    }

    private static Node GetNodeUnpackingContainers(Node root, string path)
    {
        string[] segments = path.Split(NodeSystem.PathSeparator);

        Node currentNode = root;
        foreach (string segment in segments) {
            if (currentNode.Extension == ".aar") {
                currentNode.TransformWith(new Binary2Alar());
            }

            currentNode = currentNode.Children[segment];
        }

        return currentNode;
    }

    private static Node CreateInputNode(string filePath)
    {
        DataStream importedStream = DataStreamFactory.FromArray(ImportedData);
        var input = new Node(Path.GetFileName(filePath), new BinaryFormat(importedStream));

        // Recreate folder as they will be created when importing
        var root = new Node("root");
        string? directoriesPath = Path.GetDirectoryName(filePath);
        if (directoriesPath is not null) {
            NodeFactory.CreateContainersForChild(root, directoriesPath, input);
        }

        return input;
    }
}
