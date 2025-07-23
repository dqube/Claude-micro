using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace BuildingBlocks.Infrastructure.Logging;

public class LoggerService : ILoggerService
{
    private readonly ILogger<LoggerService> _logger;

    public LoggerService(ILogger<LoggerService> logger)
    {
        _logger = logger;
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogTrace(string message, params object[] args)
    {
        _logger.LogTrace(message, args);
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogDebug(string message, params object[] args)
    {
        _logger.LogDebug(message, args);
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogError(string message, params object[] args)
    {
        _logger.LogError(message, args);
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogError(Exception exception, string message, params object[] args)
    {
        _logger.LogError(exception, message, args);
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogCritical(string message, params object[] args)
    {
        _logger.LogCritical(message, args);
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogCritical(Exception exception, string message, params object[] args)
    {
        _logger.LogCritical(exception, message, args);
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _logger.BeginScope(state);
    }
}