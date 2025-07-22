using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using BuildingBlocks.API.Health.Reporters;
using BuildingBlocks.API.Health.Extensions;

namespace BuildingBlocks.API.Extensions.ServiceCollection;

public static class HealthCheckExtensions
{
    public static WebApplication UseApiHealthChecks(this WebApplication app)
    {
        app.MapApiHealthChecks();
        return app;
    }

    public static WebApplication UseHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = JsonHealthReporter.WriteResponseAsync
        });
        
        return app;
    }

    public static WebApplication UseHealthChecks(this WebApplication app, string pattern)
    {
        app.MapHealthChecks(pattern, new HealthCheckOptions
        {
            ResponseWriter = JsonHealthReporter.WriteResponseAsync
        });
        
        return app;
    }

    public static WebApplication UseHealthChecks(this WebApplication app, string pattern, HealthCheckOptions options)
    {
        app.MapHealthChecks(pattern, options);
        return app;
    }

    public static WebApplication UseDetailedHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = JsonHealthReporter.WriteResponseAsync,
            AllowCachingResponses = false
        });
        
        app.MapHealthChecks("/health/detailed", new HealthCheckOptions
        {
            ResponseWriter = WriteDetailedHealthResponse,
            AllowCachingResponses = false
        });
        
        return app;
    }

    public static WebApplication UseKubernetesHealthChecks(this WebApplication app)
    {
        // Liveness probe - basic check that app is running
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false, // No checks, just return healthy if app is running
            ResponseWriter = SimpleHealthReporter.WriteResponseAsync
        });
        
        // Readiness probe - check dependencies are ready
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = SimpleHealthReporter.WriteResponseAsync
        });
        
        // Startup probe - check if app has started
        app.MapHealthChecks("/health/startup", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("startup"),
            ResponseWriter = SimpleHealthReporter.WriteResponseAsync
        });
        
        return app;
    }

    public static WebApplication UseSimpleHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = SimpleHealthReporter.WriteResponseAsync
        });
        
        return app;
    }

    public static WebApplication UseHealthCheckUI(this WebApplication app)
    {
        // This would integrate with a health check UI package if available
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = JsonHealthReporter.WriteResponseAsync
        });
        
        // Placeholder for health check UI
        app.MapGet("/health-ui", () => Results.Content(
            "<html><body><h1>Health Check UI</h1><p>Visit <a href='/health'>/health</a> for health status</p></body></html>",
            "text/html"));
        
        return app;
    }

    public static WebApplication UseConditionalHealthChecks(this WebApplication app, Func<HealthCheckContext, bool> predicate)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = predicate,
            ResponseWriter = JsonHealthReporter.WriteResponseAsync
        });
        
        return app;
    }

    public static WebApplication UseTaggedHealthChecks(this WebApplication app, params string[] tags)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = check => tags.Any(tag => check.Tags.Contains(tag)),
            ResponseWriter = JsonHealthReporter.WriteResponseAsync
        });
        
        return app;
    }

    private static async Task WriteDetailedHealthResponse(Microsoft.AspNetCore.Http.HttpContext context, HealthReport report)
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
            }),
            summary = new
            {
                healthy = report.Entries.Count(e => e.Value.Status == HealthStatus.Healthy),
                degraded = report.Entries.Count(e => e.Value.Status == HealthStatus.Degraded),
                unhealthy = report.Entries.Count(e => e.Value.Status == HealthStatus.Unhealthy)
            }
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }
}