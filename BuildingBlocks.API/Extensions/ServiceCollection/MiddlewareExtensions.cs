using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using BuildingBlocks.API.Middleware.ErrorHandling;
using BuildingBlocks.API.Middleware.Logging;
using BuildingBlocks.API.Middleware.Security;
using BuildingBlocks.API.Configuration.Options;

namespace BuildingBlocks.API.Extensions.ServiceCollection;

public static class MiddlewareExtensions
{
    public static WebApplication UseApiMiddleware(this WebApplication app)
    {
        // Add middleware in the correct order
        app.UseCorrelationId();
        app.UseMiddlewareSecurityHeaders();
        app.UseGlobalExceptionHandler();
        app.UseRequestLogging();
        
        return app;
    }

    public static WebApplication UseCorrelationId(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        return app;
    }

    public static WebApplication UseMiddlewareSecurityHeaders(this WebApplication app)
    {
        var securityOptions = app.Services.GetService<IOptions<SecurityHeadersOptions>>()?.Value ?? new SecurityHeadersOptions();
        app.UseMiddleware<SecurityHeadersMiddleware>(securityOptions);
        return app;
    }

    public static WebApplication UseMiddlewareSecurityHeaders(this WebApplication app, SecurityHeadersOptions options)
    {
        app.UseMiddleware<SecurityHeadersMiddleware>(options);
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

    public static WebApplication UseApiRateLimiting(this WebApplication app)
    {
        var rateLimitOptions = app.Services.GetService<IOptions<RateLimitingOptions>>()?.Value;
        
        if (rateLimitOptions?.Enabled == true)
        {
            app.UseRateLimiter();
        }
        
        return app;
    }

    public static WebApplication UseCustomRateLimiting(this WebApplication app)
    {
        app.UseMiddleware<RateLimitingMiddleware>();
        return app;
    }

    public static WebApplication UseMiddlewarePipeline(this WebApplication app)
    {
        // Complete middleware pipeline in correct order
        app.UseForwardedHeaders();
        app.UseCorrelationId();
        app.UseMiddlewareSecurityHeaders();
        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseApiRateLimiting();
        app.UseGlobalExceptionHandler();
        app.UseRequestLogging();
        
        return app;
    }

    public static WebApplication UseDevelopmentMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseGlobalExceptionHandler();
        }
        
        return app;
    }

    public static WebApplication UseProductionMiddleware(this WebApplication app)
    {
        app.UseForwardedHeaders();
        app.UseCorrelationId();
        app.UseMiddlewareSecurityHeaders();
        app.UseHttpsRedirection();
        app.UseHsts();
        
        return app;
    }

    public static WebApplication UseApiCors(this WebApplication app)
    {
        var corsOptions = app.Services.GetService<IOptions<CorsOptions>>()?.Value;
        var policyName = corsOptions?.PolicyName ?? "DefaultCorsPolicy";
        
        app.UseCors(policyName);
        
        return app;
    }

    public static WebApplication UseApiCors(this WebApplication app, string policyName)
    {
        app.UseCors(policyName);
        return app;
    }
}