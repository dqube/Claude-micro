using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Asp.Versioning;

namespace BuildingBlocks.API.Versioning.Extensions;

public static class ApiVersioningExtensions
{
    public static IServiceCollection AddApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("version"),
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

    public static WebApplication UseApiVersioning(this WebApplication app)
    {
        app.UseApiVersioning();
        return app;
    }
}

public static class VersionedEndpointExtensions
{
    public static RouteGroupBuilder MapVersionedGroup(
        this IEndpointRouteBuilder endpoints,
        string prefix,
        int majorVersion,
        int minorVersion = 0)
    {
        return endpoints.MapGroup($"/v{majorVersion}/{prefix}");
    }
}