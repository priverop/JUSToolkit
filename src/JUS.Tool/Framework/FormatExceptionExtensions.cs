using System.Runtime.CompilerServices;

namespace JUS.Tool.Framework;

internal static class FrameworkExtensions
{
    extension(FormatException)
    {
        public static void ThrowIfNotEqual<T>(T value, T expected, [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : IComparable<T>
        {
            if (value.CompareTo(expected) != 0) {
                throw new FormatException($"Expected: {expected} for {paramName}, but got: {value}");
            }
        }
    }
}
