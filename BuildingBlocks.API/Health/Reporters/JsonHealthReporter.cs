using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace BuildingBlocks.API.Health.Reporters;

public static class JsonHealthReporter
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    public static Task WriteResponse(HttpContext context, HealthReport report)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(report);
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            entries = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                duration = entry.Value.Duration.TotalMilliseconds,
                description = entry.Value.Description,
                exception = entry.Value.Exception?.Message,
                data = entry.Value.Data
            })
        };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
    }
}

public static class SimpleHealthReporter
{
    public static Task WriteResponse(HttpContext context, HealthReport report)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(report);
        context.Response.ContentType = "text/plain";
        return context.Response.WriteAsync(report.Status.ToString());
    }
}