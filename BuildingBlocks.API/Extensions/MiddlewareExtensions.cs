using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BuildingBlocks.API.Middleware.ErrorHandling;
using BuildingBlocks.API.Middleware.Logging;
using BuildingBlocks.API.Middleware.Security;

namespace BuildingBlocks.API.Extensions;

public static class MiddlewareExtensions
{
    public static IServiceCollection AddApiMiddleware(this IServiceCollection services)
    {
        services.AddScoped<GlobalExceptionMiddleware>();
        services.AddScoped<RequestLoggingMiddleware>();
        services.AddScoped<CorrelationIdMiddleware>();
        services.AddScoped<SecurityHeadersMiddleware>();
        services.AddScoped<RateLimitingMiddleware>();

        return services;
    }

    public static WebApplication UseApiMiddleware(this WebApplication app)
    {
        // Order is important - correlation ID first, then logging, then exception handling
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseMiddleware<SecurityHeadersMiddleware>();

        return app;
    }

    public static WebApplication UseGlobalExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        return app;
    }

    public static WebApplication UseRequestLogging(this WebApplication app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();
        return app;
    }

    public static WebApplication UseCorrelationId(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        return app;
    }

    public static WebApplication UseSecurityHeaders(this WebApplication app)
    {
        app.UseMiddleware<SecurityHeadersMiddleware>();
        return app;
    }

    public static WebApplication UseApiRateLimiting(this WebApplication app)
    {
        app.UseMiddleware<RateLimitingMiddleware>();
        return app;
    }

    public static WebApplication UseCustomMiddleware<T>(this WebApplication app) where T : class
    {
        app.UseMiddleware<T>();
        return app;
    }

    public static WebApplication UseMiddlewarePipeline(this WebApplication app, bool includeRateLimiting = true)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseMiddleware<SecurityHeadersMiddleware>();

        if (includeRateLimiting)
        {
            app.UseMiddleware<RateLimitingMiddleware>();
        }

        return app;
    }

    public static WebApplication UseProductionMiddleware(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseMiddleware<SecurityHeadersMiddleware>();
            app.UseMiddleware<RateLimitingMiddleware>();
        }

        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();

        return app;
    }

    public static WebApplication UseDevelopmentMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            // More detailed logging in development
            app.UseMiddleware<RequestLoggingMiddleware>();
        }

        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();

        return app;
    }
}