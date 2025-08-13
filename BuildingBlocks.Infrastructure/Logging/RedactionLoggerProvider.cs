using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
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

        // Create a redacted formatter that intercepts the original message and redacts it
        var redactedFormatter = new Func<TState, Exception?, string>((s, ex) =>
        {
            var originalMessage = formatter(s, ex);
            
            // Check if the original state contains sensitive data that needs redaction
            if (s is IEnumerable<KeyValuePair<string, object?>> kvpState)
            {
                var needsRedaction = false;
                foreach (var kvp in kvpState)
                {
                    if (kvp.Value != null)
                    {
                        // Check if any parameter has sensitive field names or values
                        if (_redactionService.IsSensitiveField(kvp.Key) || 
                            _redactionService.ContainsSensitiveData(kvp.Value.ToString() ?? string.Empty) ||
                            kvp.Value.ToString()?.Contains("Username", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            needsRedaction = true;
                            break;
                        }
                    }
                }
                
                if (needsRedaction)
                {
                    // Apply comprehensive redaction to the formatted message
                    return _redactionService.RedactMessage(originalMessage);
                }
            }
            
            // For non-structured logging, just redact the message directly
            return _redactionService.RedactMessage(originalMessage);
        });

        // Pass the original state but with our redacted formatter
        _innerLogger.Log(logLevel, eventId, state, exception, redactedFormatter);
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

public class CompositeRedactionLoggerProvider : ILoggerProvider
{
    private readonly List<ILoggerProvider> _innerProviders;
    private readonly IDataRedactionService _redactionService;
    private readonly ConcurrentDictionary<string, RedactionLogger> _loggers = new();

    public CompositeRedactionLoggerProvider(List<ILoggerProvider> innerProviders, IDataRedactionService redactionService)
    {
        _innerProviders = innerProviders ?? throw new ArgumentNullException(nameof(innerProviders));
        _redactionService = redactionService ?? throw new ArgumentNullException(nameof(redactionService));
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name =>
        {
            var innerLoggers = _innerProviders.Select(provider => provider.CreateLogger(name)).ToList();
            var compositeLogger = new CompositeLogger(innerLoggers);
            return new RedactionLogger(compositeLogger, _redactionService);
        });
    }

    public void Dispose()
    {
        _loggers.Clear();
        foreach (var provider in _innerProviders)
        {
            provider?.Dispose();
        }
        _innerProviders.Clear();
    }
}

public class CompositeLogger : ILogger
{
    private readonly List<ILogger> _loggers;

    public CompositeLogger(List<ILogger> loggers)
    {
        _loggers = loggers ?? throw new ArgumentNullException(nameof(loggers));
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        var scopes = _loggers.Select(logger => logger.BeginScope(state)).ToList();
        return new CompositeScope(scopes);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _loggers.Any(logger => logger.IsEnabled(logLevel));
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        foreach (var logger in _loggers)
        {
            if (logger.IsEnabled(logLevel))
            {
                logger.Log(logLevel, eventId, state, exception, formatter);
            }
        }
    }

    private class CompositeScope : IDisposable
    {
        private readonly List<IDisposable?> _scopes;

        public CompositeScope(List<IDisposable?> scopes)
        {
            _scopes = scopes;
        }

        public void Dispose()
        {
            foreach (var scope in _scopes)
            {
                scope?.Dispose();
            }
        }
    }
}

public class RedactionLoggerFactory : ILoggerFactory
{
    private readonly ILoggerFactory _innerFactory;
    private readonly IDataRedactionService _redactionService;

    public RedactionLoggerFactory(ILoggerFactory innerFactory, IDataRedactionService redactionService)
    {
        _innerFactory = innerFactory ?? throw new ArgumentNullException(nameof(innerFactory));
        _redactionService = redactionService ?? throw new ArgumentNullException(nameof(redactionService));
    }

    public ILogger CreateLogger(string categoryName)
    {
        var innerLogger = _innerFactory.CreateLogger(categoryName);
        return new RedactionLogger(innerLogger, _redactionService);
    }

    public void AddProvider(ILoggerProvider provider)
    {
        _innerFactory.AddProvider(provider);
    }

    public void Dispose()
    {
        _innerFactory.Dispose();
    }
}

public class RedactionLogger<T> : RedactionLogger, ILogger<T>
{
    public RedactionLogger(ILogger innerLogger, IDataRedactionService redactionService) 
        : base(innerLogger, redactionService)
    {
    }
}