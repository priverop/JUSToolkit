using JUS.Tool.Fonts;
using NUnit.Framework;
using SceneGate.Ekona.Containers.Rom;
using Texim.Fonts;
using Texim.Formats.ImageSharp.Images;
using Texim.Images;
using Texim.Palettes;
using YamlDotNet.Serialization;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tests.Fonts;

[TestFixture]
public class AlFont2BinaryTests
{
    private static readonly Lazy<NitroRom> SoftwareRoot = new(TestDataBase.ReadSoftware);

    private static IEnumerable<TestCaseData> GetFonts()
    {
        if (!File.Exists(TestDataBase.SoftwareNitroRomPath)) {
            return [];
        }

        return Navigator.IterateNodes(SoftwareRoot.Value.Data.Children["font"], NavigationMode.DepthFirst)
            .Select(n => new TestCaseData(n).SetArgDisplayNames(n.Path));
    }

    [TestCaseSource(nameof(GetFonts))]
    public void GeneratesIdentical(Node fontNode)
    {
        if (fontNode.Name == "DSFont.aft") {
            Assert.Ignore("It has unmaped characters that won't get exported, so we can't get them when importing");
        } else if (fontNode.Name == "js8font.aft") {
            // It has additional 0x00 bytes at the end... we remove them so we can compare the actual font data.
            fontNode.Stream.SetLength(0x4488);
        }

        IBinary originalBinary = fontNode.GetFormatAs<IBinary>();

        AlFont font = new Binary2AlFont().Convert(originalBinary);
        BinaryFormat generatedBinary = new AlFont2Binary().Convert(font);

        bool areIdentical = generatedBinary.Stream.Compare(originalBinary.Stream);
        if (!areIdentical) {
            originalBinary.Stream.WriteTo($"expected_{fontNode.Name}");
            generatedBinary.Stream.WriteTo($"actual_{fontNode.Name}");
        }

        Assert.That(areIdentical, Is.True);
    }

    [TestCaseSource(nameof(GetFonts))]
    public void ExportRoundTripGeneratesIdentical(Node fontNode)
    {
        if (fontNode.Name == "DSFont.aft") {
            Assert.Ignore("It has unmapped characters that won't get exported, so we can't get them when importing");
        } else if (fontNode.Name == "js8font.aft") {
            // It has additional 0x00 bytes at the end... we remove them so we can compare the actual font data.
            fontNode.Stream.SetLength(0x4488);
        }

        IBinary originalBinary = fontNode.GetFormatAs<IBinary>();

        // Read
        AlFont font = new Binary2AlFont().Convert(originalBinary);

        // Export
        BinaryTextFormat fontYaml = new Font2Yaml().Convert(font);
        RgbImage fontRgbImage = new BitmapFont2RgbImage(font.Palettes.Palettes[0], BitmapFont2RgbImage.DefaultBorderColor)
            .Convert(font);
        BinaryFormat fontPngImage = new RgbImage2BinaryPng().Convert(fontRgbImage);

        // Import
        AlFont importedFont = (AlFont)new Yaml2BitmapFont<AlFont>(YamlConfigure)
            .Convert(fontYaml);

        RgbImage importedFontRgbImage = new StandardBinaryImage2RgbImage().Convert(fontPngImage);
        var imageImporter = BitmapFontImageUpdater.FromRgbImage(
            importedFontRgbImage,
            importedFont.Palettes.Palettes[0],
            BitmapFont2RgbImage.DefaultBorderColor);
        IBitmapFont updaterOutput = imageImporter.Convert(importedFont);
        Assert.That(updaterOutput, Is.SameAs(importedFont));

        // Write
        BinaryFormat generatedBinary = new AlFont2Binary().Convert(importedFont);

        Assert.That(generatedBinary.Stream.Compare(originalBinary.Stream), Is.True);
        return;

        static void YamlConfigure(DeserializerBuilder b) => b
            .WithTypeMapping<IPaletteCollection, PaletteCollection>()
            .WithTypeMapping<IPalette, Palette>();
    }

    [Test]
    public void CompareIdenticalExportedImages()
    {
        // This is the only way we can actually test DSFont, as it has unmapped chars we can't fully re-create it.
        Node fontNode = TestDataBase.ReadSoftware().Data.Children["font"].Children["DSFont.aft"];
        IBinary originalBinary = fontNode.GetFormatAs<IBinary>();

        // Read
        AlFont font = new Binary2AlFont().Convert(originalBinary);

        // Export
        BinaryTextFormat fontYaml = new Font2Yaml().Convert(font);
        RgbImage fontRgbImage = new BitmapFont2RgbImage(font.Palettes.Palettes[0], BitmapFont2RgbImage.DefaultBorderColor)
            .Convert(font);
        BinaryFormat fontPngImage = new RgbImage2BinaryPng().Convert(fontRgbImage);

        // Import
        AlFont importedFont = (AlFont)new Yaml2BitmapFont<AlFont>(YamlConfigure)
            .Convert(fontYaml);

        RgbImage importedFontRgbImage = new StandardBinaryImage2RgbImage().Convert(fontPngImage);
        var imageImporter = BitmapFontImageUpdater.FromRgbImage(
            importedFontRgbImage,
            importedFont.Palettes.Palettes[0],
            BitmapFont2RgbImage.DefaultBorderColor);
        IBitmapFont updaterOutput = imageImporter.Convert(importedFont);
        Assert.That(updaterOutput, Is.SameAs(importedFont));

        // Write
        BinaryFormat generatedBinary = new AlFont2Binary().Convert(importedFont);

        // Read and re-export for comparing
        AlFont generatedFont = new Binary2AlFont().Convert(generatedBinary);
        BinaryTextFormat generatedFontYaml = new Font2Yaml().Convert(generatedFont);
        BinaryFormat generatedFontImage = new BitmapFont2RgbImage(font.Palettes.Palettes[0], BitmapFont2RgbImage.DefaultBorderColor)
            .Convert(font)
            .ConvertWith(new RgbImage2BinaryPng());

        Assert.That(generatedFontYaml.Stream.Compare(fontYaml.Stream), Is.True);
        Assert.That(generatedFontImage.Stream.Compare(fontPngImage.Stream), Is.True);
        return;

        static void YamlConfigure(DeserializerBuilder b) => b
            .WithTypeMapping<IPaletteCollection, PaletteCollection>()
            .WithTypeMapping<IPalette, Palette>();
    }
}
