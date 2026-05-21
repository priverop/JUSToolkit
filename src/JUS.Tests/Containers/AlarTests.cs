using JUS.Tool.Containers;
using JUS.Tool.Containers.Converters;
using JUS.Tool.Utils;
using NUnit.Framework;
using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tests.Containers;

[TestFixture]
public class AlarTests
{
    private static readonly Lazy<NitroRom> Root = new(TestDataBase.ReadSoftware);

    public static IEnumerable<TestCaseData> GetLevel1Paths()
    {
        if (!File.Exists(TestDataBase.SoftwareNitroRomPath)) {
            return [];
        }

        return Navigator.IterateNodes(Root.Value.Data, NavigationMode.DepthFirst)
            .Where(n => !n.IsContainer && n.Name.EndsWith(".aar"))
            .Select(n => new TestCaseData(n.Path));
    }

    public static IEnumerable<TestCaseData> GetLevel2Paths()
    {
        if (!File.Exists(TestDataBase.SoftwareNitroRomPath)) {
            return [];
        }

        return Navigator.IterateNodes(Root.Value.Data, NavigationMode.DepthFirst)
            .Where(n => !n.IsContainer && n.Name.EndsWith(".aar"))
            .SelectMany(n => {
                NodeContainerFormat alar = new Binary2Alar().Convert(n.GetFormatAs<IBinary>()!);
                return Navigator.IterateNodes(alar.Root, NavigationMode.DepthFirst)
                    .Where(c => !c.IsContainer && c.Name.EndsWith(".aar"))
                    .Select(c => new TestCaseData(n.Path, c.Path));
            });
    }

    [TestCaseSource(nameof(GetLevel1Paths))]
    public void GenerateIdenticalContainerLevel1(string containerPath)
    {
        TestDataBase.IgnoreIfFileDoesNotExist(TestDataBase.SoftwareNitroRomPath);

        IBinary? original = Navigator.SearchNode(Root.Value.Data, containerPath)?.GetFormatAs<IBinary>();
        Assert.That(original, Is.Not.Null);

        AssertGeneratesIdentical(original);
    }

    [TestCaseSource(nameof(GetLevel2Paths))]
    public void GenerateIdenticalContainerLevel2(string parentContainerPath, string childContainerPath)
    {
        TestDataBase.IgnoreIfFileDoesNotExist(TestDataBase.SoftwareNitroRomPath);

        IBinary? parentBinary = Navigator.SearchNode(Root.Value.Data, parentContainerPath)
            ?.GetFormatAs<IBinary>();
        Assert.That(parentBinary, Is.Not.Null);

        NodeContainerFormat parent = new Binary2Alar().Convert(parentBinary);

        IBinary? original = Navigator.SearchNode(parent.Root, childContainerPath)
            ?.GetFormatAs<IBinary>();
        Assert.That(original, Is.Not.Null);

        AssertGeneratesIdentical(original);
    }

    private static void AssertGeneratesIdentical(IBinary original)
    {
        // decompress so we don't test the compression itself
        bool isCompressed = CompressionUtils.IsCompressed(original.Stream);
        if (isCompressed) {
            original = new LzssDecompression().Convert(original);
        }

        NodeContainerFormat container = new Binary2Alar().Convert(original);

        // TODO: convert back from a single converter
        IBinary actual = container switch {
            Alar2 alar2 => new Alar2ToBinary().Convert(alar2),
            Alar3 alar3 => new Alar3ToBinary().Convert(alar3),
            _ => throw new NotSupportedException("Unsupported format"),
        };

        byte[] originalData = new byte[(int)original.Stream.Length];
        original.Stream.Position = 0;
        original.Stream.ReadExactly(originalData);
        byte[] actualData = new byte[(int)actual.Stream.Length];
        actual.Stream.Position = 0;
        actual.Stream.ReadExactly(actualData);
        Assert.That(actualData, Is.EqualTo(originalData));
    }
}
