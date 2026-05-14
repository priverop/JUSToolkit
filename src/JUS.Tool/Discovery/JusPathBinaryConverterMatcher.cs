using System.Diagnostics.CodeAnalysis;
using JUS.Tool.Graphics.Converters;
using JUS.Tool.Texts.Converters;
using Yarhl.FileFormat;
using Yarhl.FileFormat.Discovery;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Discovery;

/// <summary>
/// Discover compatible converters for binary formats based on their path.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public sealed class JusPathBinaryConverterMatcher : IConverterMatcher
{
    private static readonly FormatLocation[] SupportedFiles = [
        // Graphics
        FormatLocation.Create<Binary2Koma>("data/bin/koma.bin"),
        FormatLocation.Create<BinaryKShape2SpriteCollection>("data/bin/kshape.bin"),
        // Texts
        FormatLocation.Create<Binary2Ability>("data/bin/ability_t.bin"),
        FormatLocation.Create<Binary2Bgm>("data/bin/bgm.bin"),
        FormatLocation.Create<Binary2BtlChr>("data/bin/chr_b_t.bin"),
        FormatLocation.Create<Binary2SuppChr>("data/bin/chr_s_t.bin"),
        FormatLocation.Create<Binary2SimpleBin>("data/bin/clearlst.bin"),
        FormatLocation.Create<Binary2Commwin>("data/bin/commwin.bin"),
        FormatLocation.Create<Binary2Demo>("data/bin/demo.bin"),
        FormatLocation.Create<Binary2SimpleBin>("data/bin/infoname.bin"),
        FormatLocation.Create<Binary2Komatxt>("data/bin/komatxt.bin"),
        FormatLocation.Create<Binary2Location>("data/bin/location.bin"),
        FormatLocation.Create<Binary2Piece>("data/bin/piece.bin"),
        FormatLocation.Create<Binary2Pname>("data/bin/pname.bin"),
        FormatLocation.Create<Binary2Rulemess>("data/bin/rulemess.bin"),
        FormatLocation.Create<Binary2Stage>("data/bin/stage.bin"),
        FormatLocation.Create<Binary2SimpleBin>("data/bin/title.bin"),
        FormatLocation.Create<Binary2Tutorial>("data/deckmake/tutorial.bin"),
        FormatLocation.Create<Binary2JGalaxyComplex>("data/jgalaxy/jgalaxy.aar/jgalaxy/jgalaxy.bin"),
        FormatLocation.Create<Binary2JGalaxySimple>("data/jgalaxy/jgalaxy.aar/jgalaxy/battle.bin"),
        FormatLocation.Create<Binary2JGalaxySimple>("data/jgalaxy/jgalaxy.aar/jgalaxy/mission.bin"),
        FormatLocation.Create<Binary2JQuiz>("data/jquiz/jquiz_pack.aar/jquiz/jquiz.bin"),
    ];

    /// <inheritdoc />
    public ConverterMatcherResult Match(Node input, ConverterMatcherContext context)
    {
        if (input.Format is not IBinary) {
            return ConverterMatcherResult.Incompatible();
        }

        // Check the game code and get the root node.
        bool? compatibleSoftware = SupportedSoftware.IsFromCompatibleSoftware(input, out Node root);
        if (compatibleSoftware == false) {
            return ConverterMatcherResult.Incompatible();
        }

        // Build the relative path (once) and compare against support formats
        // If we can't determine the nitrorom root, root will be the first node.
        string relativePath = input.Path.Substring(root.Path.Length + 1);
        FormatLocation? compatibleFormat = SupportedFiles
            .FirstOrDefault(x => x.Path == relativePath);
        if (compatibleFormat is null) {
            return ConverterMatcherResult.Incompatible();
        }

        // "should be" if header match, but we can't verify the game code
        MatchingConfidence level = compatibleSoftware == true ? MatchingConfidence.Confident : MatchingConfidence.ShouldBe;
        IConverter converter = compatibleFormat.ConverterFactory();
        return new ConverterMatcherResult(level, compatibleFormat.ConverterType, converter);
    }

    private sealed record FormatLocation(string Path, Type ConverterType, Func<IConverter> ConverterFactory)
    {
        public static FormatLocation Create<T>(string path)
            where T : class, IConverter, new()
        {
            return new FormatLocation(path, typeof(T), () => new T());
        }
    }
}
