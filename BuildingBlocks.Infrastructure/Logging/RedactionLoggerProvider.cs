using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace BuildingBlocks.Infrastructure.Logging;

public class RedactionLoggerProvider : ILoggerProvider
{
    private readonly ILoggerProvider _innerProvider;
    private readonly IDataRedactionService _redactionService;
    private readonly ConcurrentDictionary<string, RedactionLogger> _loggers = new();

    public RedactionLoggerProvider(ILoggerProvider innerProvider, IDataRedactionService redactionService)
    {
        _innerProvider = innerProvider ?? throw new ArgumentNullException(nameof(innerProvider));
        _redactionService = redactionService ?? throw new ArgumentNullException(nameof(redactionService));
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name =>
        {
            var innerLogger = _innerProvider.CreateLogger(name);
            return new RedactionLogger(innerLogger, _redactionService);
        });
    }

    public void Dispose()
    {
        _loggers.Clear();
        _innerProvider?.Dispose();
    }
}

public class RedactionLogger : ILogger
{
    private readonly ILogger _innerLogger;
    private readonly IDataRedactionService _redactionService;

    public RedactionLogger(ILogger innerLogger, IDataRedactionService redactionService)
    {
        _innerLogger = innerLogger ?? throw new ArgumentNullException(nameof(innerLogger));
        _redactionService = redactionService ?? throw new ArgumentNullException(nameof(redactionService));
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        // Redact scope state if it's sensitive
        var redactedState = state;
        if (state is string stringState)
        {
            redactedState = (TState)(object)_redactionService.RedactMessage(stringState);
        }
        else if (state is Dictionary<string, object?> dictState)
        {
            redactedState = (TState)(object)_redactionService.RedactProperties(dictState);
        }

        return _innerLogger.BeginScope(redactedState);
    }

    public bool IsEnabled(LogLevel logLevel) => _innerLogger.IsEnabled(logLevel);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        // Create a redacted formatter
        var redactedFormatter = new Func<TState, Exception?, string>((s, ex) =>
        {
            var originalMessage = formatter(s, ex);
            return _redactionService.RedactMessage(originalMessage);
        });

        // Redact state if it contains sensitive data
        var redactedState = state;
        if (state is string stringState)
        {
            redactedState = (TState)(object)_redactionService.RedactMessage(stringState);
        }
        else if (state is Dictionary<string, object?> dictState)
        {
            redactedState = (TState)(object)_redactionService.RedactProperties(dictState);
        }
        else if (state is IEnumerable<KeyValuePair<string, object?>> kvpState)
        {
            var dict = kvpState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var redactedDict = _redactionService.RedactProperties(dict);
            redactedState = (TState)(object)redactedDict.AsEnumerable();
        }

        _innerLogger.Log(logLevel, eventId, redactedState, exception, redactedFormatter);
    }
}

public static class RedactionLoggerExtensions
{
    public static ILoggingBuilder AddRedaction(this ILoggingBuilder builder)
    {
        return builder.AddRedaction(options => { });
    }

    public static ILoggingBuilder AddRedaction(this ILoggingBuilder builder, Action<RedactionOptions> configure)
    {
        var options = new RedactionOptions();
        configure(options);

        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<IDataRedactionService, DataRedactionService>();

        return builder;
    }

    public static ILoggingBuilder AddRedaction(this ILoggingBuilder builder, RedactionOptions options)
    {
        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<IDataRedactionService, DataRedactionService>();

        return builder;
    }
}