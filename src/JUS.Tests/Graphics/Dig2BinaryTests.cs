using JUS.Tool.Graphics;
using JUS.Tool.Graphics.Converters;
using JUS.Tool.Utils;
using NUnit.Framework;
using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tests.Graphics;

[TestFixture]
public class Dig2BinaryTests
{
    private static IEnumerable<TestCaseData> GetDsigNodes()
    {
        NitroRom? unpackedRoot = TestDataBase.UnpackedRoot.Value;
        if (unpackedRoot is null) {
            return [];
        }

        return Navigator.IterateNodes(unpackedRoot.Data, NavigationMode.DepthFirst)
            .Where(n => n.Extension == ".dig")
            .Select(n => new TestCaseData(n).SetArgDisplayNames(n.Path));
    }

    private static IEnumerable<TestCaseData> GetDstxNodes()
    {
        NitroRom? unpackedRoot = TestDataBase.UnpackedRoot.Value;
        if (unpackedRoot is null) {
            return [];
        }

        return Navigator.IterateNodes(unpackedRoot.Data, NavigationMode.DepthFirst)
            .Where(n => n.Extension == ".dtx")
            .Select(n => new TestCaseData(n).SetArgDisplayNames(n.Path));
    }

    [TestCaseSource(nameof(GetDsigNodes))]
    public void TwoWaysIdenticalDigStream(Node node)
    {
        IBinary originalBinary = node.GetFormatAs<IBinary>();
        if (CompressionUtils.IsCompressed(originalBinary.Stream)) {
            originalBinary = new LzssDecompression().Convert(originalBinary);
        }

        Dig dig = new Binary2Dig().Convert(originalBinary);
        BinaryFormat generatedStream = new Dig2Binary().Convert(dig);

        bool areIdentical = generatedStream.Stream.Compare(originalBinary.Stream);
#if DEBUG
        if (!areIdentical) {
            TestDataBase.WriteFailedData(generatedStream.Stream, $"actual_{node.Name}");
            TestDataBase.WriteFailedData(originalBinary.Stream, $"expected_{node.Name}");
        }
#endif

        Assert.That(areIdentical, Is.True);
    }

    [TestCaseSource(nameof(GetDstxNodes))]
    public void TwoWaysIdenticalInsideDstx(Node dstx)
    {
        IBinary dstxBinary = dstx.GetFormatAs<IBinary>();
        if (CompressionUtils.IsCompressed(dstxBinary.Stream)) {
            dstxBinary = new LzssDecompression().Convert(dstxBinary);
        }

        // exclude 0x83 which has the dig in another file
        dstx.Stream.Position = 5;
        bool hasDsig = (dstx.Stream.ReadByte() & 0x80) == 0;
        if (!hasDsig) {
            Assert.Pass("DSIG in separate file");
        }

        using BinaryFormat originalDsig = ExtractDsigData(dstxBinary.Stream);
        Dig dig = new Binary2Dig().Convert(originalDsig);
        BinaryFormat generatedStream = new Dig2Binary().Convert(dig);

        bool areIdentical = generatedStream.Stream.Compare(originalDsig.Stream);
#if DEBUG
        if (!areIdentical) {
            TestDataBase.WriteFailedData(generatedStream.Stream, $"actual_{dstx.Name}");
            TestDataBase.WriteFailedData(originalDsig.Stream, $"expected_{dstx.Name}");
        }
#endif

        Assert.That(areIdentical, Is.True);

        return;
        static BinaryFormat ExtractDsigData(Stream dstx)
        {
            var reader = new DataReader(dstx);
            reader.Stream.Position = 0x8;
            ushort dsigOffset = reader.ReadUInt16();
            return new BinaryFormat(dstx.Slice(dsigOffset));
        }
    }
}
