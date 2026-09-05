using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace JUS.Tool.Utils;

/// <summary>
/// Logger factory for the framework.
/// </summary>
public static class JusLoggerFactory
{
    /// <summary>
    /// Gets or sets the logger factory to use for the framework.
    /// </summary>
    public static ILoggerFactory Instance {
        get => field ??= NullLoggerFactory.Instance;
        set;
    }
}
