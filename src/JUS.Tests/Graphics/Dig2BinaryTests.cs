using JUS.Tool.Graphics;
using JUS.Tool.Graphics.Converters;
using JUS.Tool.Utils;
using NUnit.Framework;
using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tests.Graphics;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class Dig2BinaryTests
{
    private const int CompressedMargin = 512;

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

        try {
            if (dig.DataFormat is DigDataFormat.CompressedBlocks or DigDataFormat.CompressedImage) {
                AssertEquivalentCompressedPixels(originalBinary, dig, generatedStream);
            } else {
                Assert.That(
                    generatedStream.Stream.Compare(originalBinary.Stream),
                    Is.True,
                    $"DSIG (v{dig.Version}/{dig.DataFormat}) are not identical");
            }
        } catch {
#if DEBUG
            TestDataBase.WriteFailedData(generatedStream.Stream, $"actual_{node.Name}");
            TestDataBase.WriteFailedData(originalBinary.Stream, $"expected_{node.Name}");
#endif
            throw;
        }
    }

    [TestCaseSource(nameof(GetDstxNodes))]
    public void TwoWaysIdenticalInsideDstx(Node dstx)
    {
        IBinary dstxBinary = dstx.GetFormatAs<IBinary>();
        if (CompressionUtils.IsCompressed(dstxBinary.Stream)) {
            dstxBinary = new LzssDecompression().Convert(dstxBinary);
        }

        // exclude 0x83 which has the dig in another file
        var reader = new DataReader(dstx.Stream);
        reader.Stream.Position = 5;
        byte dstxType = reader.ReadByte();
        bool hasDsig = (dstxType & 0x80) == 0;
        if (!hasDsig) {
            Assert.Pass("DSIG in separate file");
        }

        bool supportAlpha = false;
        if (dstxType is 4) {
            reader.Stream.Position = 0x0A;
            supportAlpha = reader.ReadUInt16() == 1;
        }

        using BinaryFormat originalDsig = ExtractDsigData(dstxBinary.Stream);
        Dig dig = new Binary2Dig(supportAlpha).Convert(originalDsig);
        BinaryFormat generatedStream = new Dig2Binary().Convert(dig);

        try {
            if (dig.DataFormat is DigDataFormat.CompressedBlocks or DigDataFormat.CompressedImage) {
                AssertEquivalentCompressedPixels(originalDsig, dig, generatedStream);
            } else {
                Assert.That(
                    generatedStream.Stream.Compare(originalDsig.Stream),
                    Is.True,
                    $"DSIG (v{dig.Version}/{dig.DataFormat}) are not identical");
            }
        } catch {
#if DEBUG
            TestDataBase.WriteFailedData(generatedStream.Stream, $"actual_{dstx.Name}");
            TestDataBase.WriteFailedData(originalDsig.Stream, $"expected_{dstx.Name}");
#endif
            throw;
        }

        return;
        static BinaryFormat ExtractDsigData(Stream dstx)
        {
            var reader = new DataReader(dstx);
            reader.Stream.Position = 0x8;
            ushort dsigOffset = reader.ReadUInt16();
            return new BinaryFormat(dstx.Slice(dsigOffset));
        }
    }

    private static void AssertEquivalentCompressedPixels(IBinary original, Dig dig, BinaryFormat generated)
    {
        // We can't compare compressed pixels because LZSS algorithm is different
        // Assert data before compression except in compressed blocks because the header has the compression length
        if (dig.DataFormat is DigDataFormat.CompressedImage || generated.Stream.Length == original.Stream.Length) {
            int pixelsDataOffset = 0xC + (dig.Palettes.Sum(p => p.Colors.Count) * 2);
            if (dig.Version == 2 && dig.DataFormat is DigDataFormat.CompressedBlocks) {
                pixelsDataOffset += 4;
            }

            DataStream originalComparable = original.Stream.Slice(0, pixelsDataOffset);
            DataStream generatedComparable = generated.Stream.Slice(0, pixelsDataOffset);
            Assert.That(generatedComparable.Compare(originalComparable), Is.True, "Raw data is not equal");
        }

        // Files should not be bigger as they may cause RAM issues.
        Assert.That(generated.Stream.Length, Is.LessThanOrEqualTo(original.Stream.Length + CompressedMargin));

        // Deserialize and check we have the same pixels (assuming deserializer is perfect)
        Dig generatedDig = new Binary2Dig().Convert(generated);
        Assert.That(generatedDig, Is.EqualTo(dig).UsingPropertiesComparer());
    }
}
