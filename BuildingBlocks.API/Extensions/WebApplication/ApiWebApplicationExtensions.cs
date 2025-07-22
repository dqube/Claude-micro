using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using BuildingBlocks.API.Middleware.ErrorHandling;
using BuildingBlocks.API.Middleware.Logging;
using BuildingBlocks.API.OpenApi.Configuration;

namespace BuildingBlocks.API.Extensions.WebApp;

public static class ApiWebApplicationExtensions
{
    public static WebApplication UseBuildingBlocksApi(
        this WebApplication app,
        IConfiguration? configuration = null)
    {
        // Add essential middleware
        app.UseApiMiddleware();
        
        // Add authentication and authorization
        app.UseApiSecurity();
        
        // Add documentation
        app.UseScalarDocumentation(configuration);
        
        // Add health checks
        app.UseApiHealthChecks();

        return app;
    }

    public static WebApplication UseApiMiddleware(this WebApplication app)
    {
        // CORS (must be before authentication)
        app.UseCors();
        
        // Rate limiting (commented out due to package availability)
        // app.UseRateLimiter();
        
        // Custom middleware
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();

        return app;
    }

    public static WebApplication UseApiSecurity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static WebApplication UseApiHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        
        // Add detailed health check endpoint in development
        if (app.Environment.IsDevelopment())
        {
            app.MapHealthChecks("/health/details", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    var response = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description,
                            duration = entry.Value.Duration.TotalMilliseconds
                        }),
                        totalDuration = report.TotalDuration.TotalMilliseconds
                    };

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    }));
                }
            });
        }

        return app;
    }

    public static WebApplication UseSecurityHeaders(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.Use(async (context, next) =>
            {
                // Security headers
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                context.Response.Headers["Content-Security-Policy"] = "default-src 'self'";

                await next();
            });
        }

        return app;
    }
}