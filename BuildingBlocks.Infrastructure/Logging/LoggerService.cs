using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace BuildingBlocks.Infrastructure.Logging;

public class LoggerService : ILoggerService
{
    private readonly ILogger<LoggerService> _logger;
    private static readonly ActivitySource ActivitySource = new("BuildingBlocks.Infrastructure.Logging");

    public LoggerService(ILogger<LoggerService> logger)
    {
        _logger = logger;
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogTrace(string message, params object[] args)
    {
        using var activity = ActivitySource.StartActivity("LogTrace");
        activity?.SetTag("log.level", "Trace");
        _logger.LogTrace(message, args);
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogDebug(string message, params object[] args)
    {
        using var activity = ActivitySource.StartActivity("LogDebug");
        activity?.SetTag("log.level", "Debug");
        _logger.LogDebug(message, args);
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogInformation(string message, params object[] args)
    {
        using var activity = ActivitySource.StartActivity("LogInformation");
        activity?.SetTag("log.level", "Information");
        _logger.LogInformation(message, args);
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogWarning(string message, params object[] args)
    {
        using var activity = ActivitySource.StartActivity("LogWarning");
        activity?.SetTag("log.level", "Warning");
        _logger.LogWarning(message, args);
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogError(string message, params object[] args)
    {
        using var activity = ActivitySource.StartActivity("LogError");
        activity?.SetTag("log.level", "Error");
        _logger.LogError(message, args);
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogError(Exception exception, string message, params object[] args)
    {
        using var activity = ActivitySource.StartActivity("LogError");
        activity?.SetTag("log.level", "Error");
        activity?.SetTag("exception.type", exception.GetType().Name);
        activity?.SetTag("exception.message", exception.Message);
        _logger.LogError(exception, message, args);
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogCritical(string message, params object[] args)
    {
        using var activity = ActivitySource.StartActivity("LogCritical");
        activity?.SetTag("log.level", "Critical");
        _logger.LogCritical(message, args);
    }

    [SuppressMessage("LoggingGenerator", "CA1848:Use the LoggerMessage delegates", Justification = "This is a dynamic logging wrapper service")]
    [SuppressMessage("LoggingGenerator", "CA2254:Template should not vary between calls", Justification = "This is a dynamic logging wrapper service")]
    public void LogCritical(Exception exception, string message, params object[] args)
    {
        using var activity = ActivitySource.StartActivity("LogCritical");
        activity?.SetTag("log.level", "Critical");
        activity?.SetTag("exception.type", exception.GetType().Name);
        activity?.SetTag("exception.message", exception.Message);
        _logger.LogCritical(exception, message, args);
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _logger.BeginScope(state);
    }
}