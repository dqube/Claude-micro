using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.API.Middleware.Logging;

public class RequestLoggingMiddleware
{
    private static readonly Action<ILogger, string, string, string, Exception?> LogRequestStart =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Information,
            new EventId(2001, nameof(LogRequestStart)),
            "Starting request {Method} {Path} - CorrelationId: {CorrelationId}");

    private static readonly Action<ILogger, string, string, int, long, string, Exception?> LogRequestEnd =
        LoggerMessage.Define<string, string, int, long, string>(
            LogLevel.Information,
            new EventId(2002, nameof(LogRequestEnd)),
            "Completed request {Method} {Path} - Status: {StatusCode}, Duration: {Duration}ms - CorrelationId: {CorrelationId}");
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(logger);
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.TraceIdentifier;

        // Log request
        LogRequestStart(_logger, context.Request.Method, context.Request.Path, correlationId, null);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            // Log response
            LogRequestEnd(
                _logger,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                correlationId,
                null);
        }
    }
}