using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Asp.Versioning;

namespace BuildingBlocks.API.Extensions;

public static class VersioningExtensions
{
    public static IServiceCollection AddApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("version"),
                new QueryStringApiVersionReader("v"),
                new HeaderApiVersionReader("X-Version"),
                new UrlSegmentApiVersionReader()
            );
        })
        .AddApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    public static RouteGroupBuilder MapVersionedApiGroup(
        this IEndpointRouteBuilder endpoints,
        string prefix,
        int majorVersion,
        int minorVersion = 0)
    {
        var version = new ApiVersion(majorVersion, minorVersion);
        var group = endpoints.MapGroup($"/api/v{majorVersion}/{prefix}");
        group.WithOpenApi();
        return group;
    }

    public static RouteHandlerBuilder WithApiVersion(
        this RouteHandlerBuilder builder,
        int majorVersion,
        int minorVersion = 0)
    {
        var version = new ApiVersion(majorVersion, minorVersion);
        return builder.WithMetadata(version);
    }

    public static RouteGroupBuilder WithApiVersion(
        this RouteGroupBuilder group,
        int majorVersion,
        int minorVersion = 0)
    {
        var version = new ApiVersion(majorVersion, minorVersion);
        return group.WithMetadata(version);
    }

    public static RouteHandlerBuilder MapVersionedGet(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate handler,
        int majorVersion,
        int minorVersion = 0)
    {
        return endpoints.MapGet(pattern, handler)
            .WithApiVersion(majorVersion, minorVersion)
            .WithOpenApi();
    }

    public static RouteHandlerBuilder MapVersionedPost(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate handler,
        int majorVersion,
        int minorVersion = 0)
    {
        return endpoints.MapPost(pattern, handler)
            .WithApiVersion(majorVersion, minorVersion)
            .WithOpenApi();
    }

    public static RouteHandlerBuilder MapVersionedPut(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate handler,
        int majorVersion,
        int minorVersion = 0)
    {
        return endpoints.MapPut(pattern, handler)
            .WithApiVersion(majorVersion, minorVersion)
            .WithOpenApi();
    }

    public static RouteHandlerBuilder MapVersionedDelete(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate handler,
        int majorVersion,
        int minorVersion = 0)
    {
        return endpoints.MapDelete(pattern, handler)
            .WithApiVersion(majorVersion, minorVersion)
            .WithOpenApi();
    }
}