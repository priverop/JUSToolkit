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

    private static IEnumerable<TestCaseData> GetLevel1Paths()
    {
        if (!File.Exists(TestDataBase.SoftwareNitroRomPath)) {
            return [];
        }

        return Navigator.IterateNodes(Root.Value.Data, NavigationMode.DepthFirst)
            .Where(n => !n.IsContainer && n.Name.EndsWith(".aar"))
            .Select(n => new TestCaseData(n).SetArgDisplayNames(n.Path));
    }

    private static IEnumerable<TestCaseData> GetLevel2Paths()
    {
        if (!File.Exists(TestDataBase.SoftwareNitroRomPath)) {
            return [];
        }

        return Navigator.IterateNodes(Root.Value.Data, NavigationMode.DepthFirst)
            .Where(n => !n.IsContainer && n.Name.EndsWith(".aar"))
            .SelectMany(n => {
                NodeContainerFormat alar = new Binary2Alar().Convert(n.GetFormatAs<IBinary>());
                return Navigator.IterateNodes(alar.Root, NavigationMode.DepthFirst)
                    .Where(c => !c.IsContainer && c.Name.EndsWith(".aar"))
                    .Select(c => new TestCaseData(c).SetArgDisplayNames($"{n.Path}{c.Path}"));
            });
    }

    [TestCaseSource(nameof(GetLevel1Paths))]
    [TestCaseSource(nameof(GetLevel2Paths))]
    public void GenerateIdenticalContainers(Node container)
    {
        IBinary original = container.GetFormatAs<IBinary>();
        AssertGeneratesIdentical(original);
    }

    private static void AssertGeneratesIdentical(IBinary original)
    {
        // decompress so we don't test the compression itself
        bool isCompressed = CompressionUtils.IsCompressed(original.Stream);
        if (isCompressed) {
            original = new LzssDecompression().Convert(original);
        }

        Alar container = new Binary2Alar().Convert(original);
        BinaryFormat actual = new AlarToBinary().Convert(container);
        Assert.That(actual, Is.Not.Null);

        byte[] originalData = new byte[(int)original.Stream.Length];
        original.Stream.Position = 0;
        original.Stream.ReadExactly(originalData);
        byte[] actualData = new byte[(int)actual.Stream.Length];
        actual.Stream.Position = 0;
        actual.Stream.ReadExactly(actualData);
        Assert.That(actualData, Is.EqualTo(originalData));
    }
}
