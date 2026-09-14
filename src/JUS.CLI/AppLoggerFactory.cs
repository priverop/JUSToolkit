using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using NLog;

namespace JUS.CLI;

internal static class AppLoggerFactory
{
    private static readonly Lazy<ILoggerFactory> Factory = new(CreateFactory);

    public static Microsoft.Extensions.Logging.LogLevel MinimumLevel {
        get;
        set {
            if (Factory.IsValueCreated) {
                throw new InvalidOperationException("Cannot change verbosity after factory initialization");
            }

            field = value;
        }
    } = Microsoft.Extensions.Logging.LogLevel.Warning;

    public static ILoggerFactory GetFactory() => Factory.Value;

    public static ILogger<T> CreateLogger<T>()
    {
        return Factory.Value.CreateLogger<T>();
    }

    public static Microsoft.Extensions.Logging.ILogger CreateLogger(string name)
    {
        return Factory.Value.CreateLogger(name);
    }

    private static ILoggerFactory CreateFactory()
    {
        LogManager.ThrowConfigExceptions = true;
        var nlogConfig = CreateNLogConfiguration();
        var nlogProviderConfig = CreateNLogProviderConfiguration();

        return LoggerFactory.Create(builder =>
            builder.AddNLog(nlogConfig, nlogProviderConfig)
                .SetMinimumLevel(MinimumLevel));
    }

    private static LoggingConfiguration CreateNLogConfiguration()
    {
        var config = new LoggingConfiguration();

        var coloredConsole = new ColoredConsoleTarget {
            Layout = "${level:uppercase=true}: ${logger:shortName=true} => ${message:withException=true}",
            AutoFlush = true,
        };
        config.AddRuleForAllLevels(coloredConsole);

        return config;
    }

    private static NLogProviderOptions CreateNLogProviderConfiguration()
    {
        return new NLogProviderOptions() {
            RemoveLoggerFactoryFilter = false,
        };
    }
}
