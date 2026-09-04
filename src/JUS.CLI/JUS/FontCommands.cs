using System.CommandLine;
using JUS.Tool.Fonts;
using Texim.Fonts;
using Texim.Formats.ImageSharp.Images;
using Texim.Images;
using Texim.Palettes;
using YamlDotNet.Serialization;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.CLI.JUS;

internal static class FontCommands
{
    public static Command CreateCommands()
    {
        var exportFont = new Command("export", "Export ALFT fonts") {
            new Option<string>("--input") { Description = "ALFT font file", Required = true },
            new Option<string>("--output") { Description = "Output directory", Required = true },
        };
        exportFont.SetAction(r => {
            string input = r.GetRequiredValue<string>("--input");
            string output = r.GetRequiredValue<string>("--output");
            ExportFont(input, output);
        });

        var importFont = new Command("import", "Import ALFT fonts") {
            new Option<string>("--yaml") { Description = "YAML font file", Required = true },
            new Option<string>("--image") { Description = "PNG font image", Required = true },
            new Option<string>("--output") { Description = "Output ALFT file", Required = true },
        };
        importFont.SetAction(r => {
            string yaml = r.GetRequiredValue<string>("--yaml");
            string image = r.GetRequiredValue<string>("--image");
            string output = r.GetRequiredValue<string>("--output");
            ImportFont(yaml, image, output);
        });

        return new Command("fonts", "Export or import fonts") {
            exportFont,
            importFont,
        };
    }

    private static void ExportFont(string inputFile, string outputDirectory)
    {
        using Node input = NodeFactory.FromFile(inputFile, FileOpenMode.Read)
            .TransformWith<Binary2AlFont>();

        AlFont font = input.GetFormatAs<AlFont>();
        font.ConvertWith(new Font2Yaml())
            .Stream.WriteTo(Path.Combine(outputDirectory, input.Name + ".yml"));

        font.ConvertWith(new BitmapFont2RgbImage(font.Palettes.Palettes[0], BitmapFont2RgbImage.DefaultBorderColor))
            .ConvertWith(new RgbImage2BinaryPng())
            .Stream.WriteTo(Path.Combine(outputDirectory, input.Name + ".png"));
    }

    private static void ImportFont(string yamlFile, string imageFile, string outputFile)
    {
        using Node fontImageNode = NodeFactory.FromFile(imageFile, FileOpenMode.Read)
            .TransformWith(new StandardBinaryImage2RgbImage());

        using Node fontNode = NodeFactory.FromFile(yamlFile, FileOpenMode.Read)
            .TransformWith(new Yaml2BitmapFont<AlFont>(YamlConfigure));

        AlFont font = fontNode.GetFormatAs<AlFont>();
        RgbImage fontImage = fontImageNode.GetFormatAs<RgbImage>();
        var imageImporter = BitmapFontImageUpdater.FromRgbImage(
            fontImage,
            font.Palettes.Palettes[0],
            BitmapFont2RgbImage.DefaultBorderColor);

        fontNode.TransformWith(imageImporter)
            .TransformWith(new AlFont2Binary())
            .Stream.WriteTo(outputFile);

        return;

        static void YamlConfigure(DeserializerBuilder b) => b
            .WithTypeMapping<IPaletteCollection, PaletteCollection>()
            .WithTypeMapping<IPalette, Palette>();
    }
}
