using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;
using BuildingBlocks.API.Configuration.Options;
using BuildingBlocks.API.Middleware.Security;

namespace BuildingBlocks.API.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddApiRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        
        var rateLimitOptions = configuration.GetSection(RateLimitingOptions.SectionName).Get<RateLimitingOptions>()
                              ?? new RateLimitingOptions();

        services.Configure<RateLimitingOptions>(configuration.GetSection(RateLimitingOptions.SectionName));
        services.AddSingleton(rateLimitOptions);

        // Register ASP.NET Core rate limiting services
        if (rateLimitOptions.Enabled)
        {
            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    // Skip rate limiting for excluded paths
                    var path = httpContext.Request.Path.Value;
                    if (path != null && rateLimitOptions.ExcludedPaths.Any(excludedPath => 
                        path.StartsWith(excludedPath, StringComparison.OrdinalIgnoreCase)))
                    {
                        return RateLimitPartition.GetNoLimiter("excluded");
                    }

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = rateLimitOptions.PermitLimit,
                            Window = rateLimitOptions.Window
                        });
                });

                options.RejectionStatusCode = 429;
            });
        }

        return services;
    }

    public static IServiceCollection AddApiRateLimiting(
        this IServiceCollection services,
        Action<RateLimitingOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);
        
        var options = new RateLimitingOptions();
        configureOptions(options);

        services.Configure<RateLimitingOptions>(opts => configureOptions(opts));
        services.AddSingleton(options);

        // Register ASP.NET Core rate limiting services
        if (options.Enabled)
        {
            services.AddRateLimiter(rateLimiterOptions =>
            {
                rateLimiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    // Skip rate limiting for excluded paths
                    var path = httpContext.Request.Path.Value;
                    if (path != null && options.ExcludedPaths.Any(excludedPath => 
                        path.StartsWith(excludedPath, StringComparison.OrdinalIgnoreCase)))
                    {
                        return RateLimitPartition.GetNoLimiter("excluded");
                    }

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = options.PermitLimit,
                            Window = options.Window
                        });
                });

                rateLimiterOptions.RejectionStatusCode = 429;
            });
        }

        return services;
    }

    public static WebApplication UseApiRateLimiting(this WebApplication app)
    {
        app.UseMiddleware<RateLimitingMiddleware>();
        return app;
    }

    public static WebApplication UseApiRateLimiting(
        this WebApplication app,
        RateLimitingOptions options)
    {
        app.UseMiddleware<RateLimitingMiddleware>();
        return app;
    }

    public static RouteHandlerBuilder WithRateLimit(
        this RouteHandlerBuilder builder,
        int permitLimit,
        TimeSpan window)
    {
        // Add metadata for specific endpoint rate limiting
        builder.WithMetadata(new RateLimitMetadata(permitLimit, window));
        return builder;
    }

    public static RouteGroupBuilder WithRateLimit(
        this RouteGroupBuilder group,
        int permitLimit,
        TimeSpan window)
    {
        group.WithMetadata(new RateLimitMetadata(permitLimit, window));
        return group;
    }

    public static RouteHandlerBuilder WithoutRateLimit(this RouteHandlerBuilder builder)
    {
        builder.WithMetadata(new NoRateLimitMetadata());
        return builder;
    }

    public static RouteGroupBuilder WithoutRateLimit(this RouteGroupBuilder group)
    {
        group.WithMetadata(new NoRateLimitMetadata());
        return group;
    }
}

public record RateLimitMetadata(int PermitLimit, TimeSpan Window);

public record NoRateLimitMetadata;