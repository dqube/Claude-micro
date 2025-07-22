using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BuildingBlocks.API.Authentication.JWT;
using BuildingBlocks.API.Authentication.ApiKey;
using BuildingBlocks.API.Middleware.ErrorHandling;
using BuildingBlocks.API.Middleware.Logging;
using BuildingBlocks.API.OpenApi.Configuration;
using BuildingBlocks.API.Validation.Extensions;

namespace BuildingBlocks.API.Extensions;

/// <summary>
/// Extension methods for configuring BuildingBlocks API services and middleware
/// </summary>
public static class ApiExtensions
{
    #region Service Collection Extensions

    /// <summary>
    /// Adds all BuildingBlocks API services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddBuildingBlocksApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add core API services
        services.AddApiCore();
        
        // Add authentication
        services.AddApiAuthentication(configuration);
        
        // Add OpenAPI documentation
        services.AddOpenApiWithAuthentication(configuration);
        
        // Add validation
        services.AddFluentValidation();
        
        // Add CORS
        services.AddApiCors(configuration);
        
        // Add rate limiting (commented out due to package availability)
        // services.AddApiRateLimiting(configuration);
        
        // Add health checks
        services.AddApiHealthChecks();

        return services;
    }

    /// <summary>
    /// Adds core API services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApiCore(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddProblemDetails();
        
        // Configure JSON options
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.SerializerOptions.WriteIndented = true;
        });

        return services;
    }

    /// <summary>
    /// Adds API authentication services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authSection = configuration.GetSection("Authentication");
        
        // Add JWT if configured
        if (authSection.GetSection("Jwt").Exists())
        {
            services.AddJwtAuthentication(configuration);
        }
        
        // Add API Key if configured
        if (authSection.GetSection("ApiKey").Exists())
        {
            services.AddApiKeyAuthentication(configuration);
        }

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Adds CORS services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApiCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var corsSettings = configuration.GetSection("Cors");
        
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                var origins = corsSettings.GetSection("AllowedOrigins").Get<string[]>();
                var methods = corsSettings.GetSection("AllowedMethods").Get<string[]>();
                var headers = corsSettings.GetSection("AllowedHeaders").Get<string[]>();

                if (origins?.Any() == true)
                {
                    builder.WithOrigins(origins);
                }
                else
                {
                    builder.AllowAnyOrigin();
                }

                if (methods?.Any() == true)
                {
                    builder.WithMethods(methods);
                }
                else
                {
                    builder.AllowAnyMethod();
                }

                if (headers?.Any() == true)
                {
                    builder.WithHeaders(headers);
                }
                else
                {
                    builder.AllowAnyHeader();
                }
            });
        });

        return services;
    }

    // Commented out due to package availability in .NET 9
    // /// <summary>
    // /// Adds rate limiting services
    // /// </summary>
    // /// <param name="services">The service collection</param>
    // /// <param name="configuration">The configuration</param>
    // /// <returns>The service collection</returns>
    // public static IServiceCollection AddApiRateLimiting(
    //     this IServiceCollection services,
    //     IConfiguration configuration)
    // {
    //     // Rate limiting implementation would go here
    //     // Can be implemented with custom middleware or third-party packages
    //     return services;
    // }

    /// <summary>
    /// Adds health check services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

        return services;
    }

    #endregion

    #region Web Application Extensions

    /// <summary>
    /// Configures the web application with BuildingBlocks API middleware and features
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="configuration">The configuration (optional)</param>
    /// <returns>The web application</returns>
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

    /// <summary>
    /// Configures API middleware
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application</returns>
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

    /// <summary>
    /// Configures API security middleware
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application</returns>
    public static WebApplication UseApiSecurity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// Configures API health check endpoints
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application</returns>
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

    /// <summary>
    /// Configures security headers middleware
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application</returns>
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

    #endregion
}
