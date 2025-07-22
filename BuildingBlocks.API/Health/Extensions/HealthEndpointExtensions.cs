using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using BuildingBlocks.API.Health.Reporters;
using System.Text.Json;

namespace BuildingBlocks.API.Health.Extensions;

public static class HealthEndpointExtensions
{
    public static IEndpointRouteBuilder MapApiHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        // Main health check endpoint
        endpoints.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = JsonHealthReporter.WriteResponseAsync,
            AllowCachingResponses = false
        });

        // Ready endpoint (for Kubernetes readiness probe)
        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = SimpleHealthReporter.WriteResponseAsync,
            AllowCachingResponses = false
        });

        // Live endpoint (for Kubernetes liveness probe)
        endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false, // No checks, just return healthy if app is running
            ResponseWriter = SimpleHealthReporter.WriteResponseAsync,
            AllowCachingResponses = false
        });

        // Detailed health check with all information
        endpoints.MapHealthChecks("/health/detailed", new HealthCheckOptions
        {
            ResponseWriter = WriteDetailedHealthResponse,
            AllowCachingResponses = false
        });

        return endpoints;
    }

    public static IEndpointRouteBuilder MapCustomHealthCheck(
        this IEndpointRouteBuilder endpoints, 
        string pattern, 
        Func<HealthCheckContext, bool>? predicate = null,
        Func<HttpContext, HealthReport, Task>? responseWriter = null)
    {
        endpoints.MapHealthChecks(pattern, new HealthCheckOptions
        {
            Predicate = predicate,
            ResponseWriter = responseWriter ?? JsonHealthReporter.WriteResponseAsync,
            AllowCachingResponses = false
        });

        return endpoints;
    }

    private static async Task WriteDetailedHealthResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            timestamp = DateTime.UtcNow,
            entries = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                duration = entry.Value.Duration.TotalMilliseconds,
                description = entry.Value.Description,
                data = entry.Value.Data,
                exception = entry.Value.Exception?.Message,
                tags = entry.Value.Tags
            })
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}