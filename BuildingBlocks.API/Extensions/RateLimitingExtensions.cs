using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.API.Configuration.Options;
using BuildingBlocks.API.Middleware.Security;

namespace BuildingBlocks.API.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddApiRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var rateLimitOptions = configuration.GetSection(RateLimitingOptions.SectionName).Get<RateLimitingOptions>()
                              ?? new RateLimitingOptions();

        services.Configure<RateLimitingOptions>(configuration.GetSection(RateLimitingOptions.SectionName));
        services.AddSingleton(rateLimitOptions);

        return services;
    }

    public static IServiceCollection AddApiRateLimiting(
        this IServiceCollection services,
        Action<RateLimitingOptions> configureOptions)
    {
        var options = new RateLimitingOptions();
        configureOptions(options);

        services.Configure<RateLimitingOptions>(opts => configureOptions(opts));
        services.AddSingleton(options);

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