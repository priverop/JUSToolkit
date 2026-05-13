using System.Diagnostics.CodeAnalysis;
using JUS.Tool.Containers.Converters;
using JUS.Tool.Graphics.Converters;
using JUS.Tool.Utils;
using Yarhl.FileFormat;
using Yarhl.FileFormat.Discovery;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Discovery;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public sealed class JusBinaryHeaderConverterMatcher : IConverterMatcher
{
    private static readonly HeaderFormat[] SupportedFormats = [
        // Containers
        HeaderFormat.Create<Binary2Alar2>("ALAR", 2, null),
        HeaderFormat.Create<Binary2Alar3>("ALAR", 3, null),
        HeaderFormat.Create<LzssDecompression>("DSCP", null, null),
        // Graphics
        HeaderFormat.Create<Binary2Almt>("ALMT", null, null),
        HeaderFormat.Create<Binary2Almt>("ALTM", null, null),
        HeaderFormat.Create<Binary2Dig>("DSIG", null, null),
        HeaderFormat.Create<BinaryToDtx3>("DSTX", 1, 3),
        HeaderFormat.Create<BinaryDtx4ToSpriteImage>("DSTX", 1, 4),
    ];

    public ConverterMatcherResult Match(Node input, ConverterMatcherContext context)
    {
        if (input.Format is not IBinary) {
            return ConverterMatcherResult.Incompatible();
        }

        // Validate the header data
        HeaderFormat? compatibleFormat = SupportedFormats
            .FirstOrDefault(f => MatchFormat(context, f));
        if (compatibleFormat is null) {
            return ConverterMatcherResult.Incompatible();
        }

        // header data match... validate this is a supported game
        bool? compatibleSoftware = SupportedSoftware.IsFromCompatibleSoftware(input, out _);
        if (compatibleSoftware == false) {
            return ConverterMatcherResult.Incompatible();
        }

        // "should be" if header match, but we can't verify the game code
        MatchingConfidence level = compatibleSoftware == true ? MatchingConfidence.Confident : MatchingConfidence.ShouldBe;
        IConverter converter = compatibleFormat.ConverterFactory();
        return new ConverterMatcherResult(level, compatibleFormat.ConverterType, converter);
    }

    private static bool MatchFormat(ConverterMatcherContext context, HeaderFormat format)
    {
        int requiredHeaderData = format switch {
            { Flags: not null } => 6,
            { Version: not null } => 5,
            _ => 4
        };
        if (context.Header.Length < requiredHeaderData) {
            return false;
        }

        if (context.Header.ReadAscii(0, 4) != format.Id) {
            return false;
        }

        if (format.Version.HasValue && context.Header.Span[4] != format.Version.Value) {
            return false;
        }

        if (format.Flags.HasValue && context.Header.Span[5] != format.Flags.Value) {
            return false;
        }

        return true;
    }

    private sealed record HeaderFormat(
        string Id,
        byte? Version,
        byte? Flags,
        Type ConverterType,
        Func<IConverter> ConverterFactory)
    {
        public static HeaderFormat Create<T>(string id, byte? version, byte? flags)
            where T : class, IConverter, new()
        {
            return new HeaderFormat(id, version, flags, typeof(T), () => new T());
        }
    }
}
