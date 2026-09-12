using JUS.Tool.Graphics.Converters;
using NUnit.Framework;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tests.Graphics;

[TestFixture]
public class BinaryDig2BitmapTests
{
    private static IEnumerable<TestCaseData> GetFiles()
    {
        string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Graphics");
        string listPath = Path.Combine(basePath, "dig.txt");
        return TestDataBase.ReadTestListFile(listPath)
            .Select(line => line.Split(','))
            .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]),
                    Path.Combine(basePath, data[2]))
                .SetName($"({data[0]}, {data[1]}, {data[2]})"));
    }

    [TestCaseSource(nameof(GetFiles))]
    public void DeserializeAndCheckFileHash(string infoPath, string digPath, string atmPath)
    {
        TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
        TestDataBase.IgnoreIfFileDoesNotExist(digPath);
        TestDataBase.IgnoreIfFileDoesNotExist(atmPath);

        var info = BinaryInfo.FromYaml(infoPath);

        using Node mapsNode = NodeFactory.FromFile(atmPath, FileOpenMode.Read);

        using Node pixelsPaletteNode = NodeFactory.FromFile(digPath, FileOpenMode.Read)
            .TransformWith(new BinaryDig2Bitmap(mapsNode));

        pixelsPaletteNode.Stream.Should().MatchInfo(info);
    }
}
