using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BuildingBlocks.API.Health.Reporters;

public static class SimpleHealthReporter
{
    public static async Task WriteResponseAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "text/plain";
        
        // Set appropriate status code
        context.Response.StatusCode = report.Status switch
        {
            HealthStatus.Healthy => StatusCodes.Status200OK,
            HealthStatus.Degraded => StatusCodes.Status200OK,
            HealthStatus.Unhealthy => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status200OK
        };

        var response = report.Status.ToString();
        await context.Response.WriteAsync(response);
    }

    public static async Task WriteDetailedResponseAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "text/plain";
        
        // Set appropriate status code
        context.Response.StatusCode = report.Status switch
        {
            HealthStatus.Healthy => StatusCodes.Status200OK,
            HealthStatus.Degraded => StatusCodes.Status200OK,
            HealthStatus.Unhealthy => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status200OK
        };

        var responseBuilder = new System.Text.StringBuilder();
        responseBuilder.AppendLine($"Status: {report.Status}");
        responseBuilder.AppendLine($"Duration: {report.TotalDuration.TotalMilliseconds}ms");
        responseBuilder.AppendLine($"Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        
        if (report.Entries.Any())
        {
            responseBuilder.AppendLine();
            responseBuilder.AppendLine("Results:");
            
            foreach (var entry in report.Entries)
            {
                responseBuilder.AppendLine($"  {entry.Key}: {entry.Value.Status} ({entry.Value.Duration.TotalMilliseconds}ms)");
                
                if (!string.IsNullOrEmpty(entry.Value.Description))
                {
                    responseBuilder.AppendLine($"    Description: {entry.Value.Description}");
                }
                
                if (entry.Value.Exception != null)
                {
                    responseBuilder.AppendLine($"    Error: {entry.Value.Exception.Message}");
                }
            }
        }

        await context.Response.WriteAsync(responseBuilder.ToString());
    }
}