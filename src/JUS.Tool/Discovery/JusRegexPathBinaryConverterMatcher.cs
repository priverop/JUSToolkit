using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using JUS.Tool.Texts.Converters;
using Yarhl.FileFormat;
using Yarhl.FileFormat.Discovery;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Discovery;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public sealed class JusRegexPathBinaryConverterMatcher : IConverterMatcher
{
    private static readonly FormatLocation[] SupportedFiles = [
        // Texts
        FormatLocation.Create<Binary2Tutorial>(@"data/battle/tutorial\d.bin"),
        FormatLocation.Create<Binary2InfoDeckDeck>(@"data/bin/InfoDeck.aar/bin/deck/\w{2}.bin"),
        FormatLocation.Create<Binary2InfoDeckInfo>(@"data/bin/InfoDeck.aar/bin/info/(\w{2}|jump).bin"),
        FormatLocation.Create<Binary2Deck>(@"data/deck/Deck\.aar/deck/\w+/\d{3}.bin"),
        FormatLocation.Create<Binary2PDeck>(@"data/deck/Deck\.aar/deck/\w+/p\d{3}.bin"),
    ];

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

        // Build the relative path (once) and match against support formats
        // If we can't determine the nitrorom root, root will be the first node.
        string relativePath = input.Path.Substring(root.Path.Length + 1);
        FormatLocation? compatibleFormat = SupportedFiles
            .FirstOrDefault(x => x.Regex.IsMatch(relativePath));
        if (compatibleFormat is null) {
            return ConverterMatcherResult.Incompatible();
        }

        // "should be" if header match, but we can't verify the game code
        MatchingConfidence level = compatibleSoftware == true ? MatchingConfidence.Confident : MatchingConfidence.ShouldBe;
        IConverter converter = compatibleFormat.ConverterFactory();
        return new ConverterMatcherResult(level, compatibleFormat.ConverterType, converter);
    }

    private sealed record FormatLocation(Regex Regex, Type ConverterType, Func<IConverter> ConverterFactory)
    {
        public static FormatLocation Create<T>(string pattern)
            where T : class, IConverter, new()
        {
            Regex regex = new(pattern, RegexOptions.Compiled);
            return new FormatLocation(regex, typeof(T), () => new T());
        }
    }
}
