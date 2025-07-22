using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Asp.Versioning;
using Asp.Versioning.Builder;

namespace BuildingBlocks.API.Versioning.Extensions;

public static class ApiVersioningExtensions
{
    public static IServiceCollection AddApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1.0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new QueryStringApiVersionReader("version"),
                new HeaderApiVersionReader("X-Version")
            );
            options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    public static IServiceCollection AddApiVersioning(this IServiceCollection services, Action<ApiVersioningOptions> configure)
    {
        services.AddApiVersioning(configure)
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }

    public static IEndpointRouteBuilder MapVersionedEndpointGroup(
        this IEndpointRouteBuilder builder,
        string prefix,
        ApiVersionSet versionSet)
    {
        var group = builder.MapGroup(prefix)
            .WithApiVersionSet(versionSet)
            .WithOpenApi();

        return group;
    }

    public static RouteGroupBuilder WithApiVersion(this RouteGroupBuilder builder, ApiVersion version)
    {
        return builder.HasApiVersion(version);
    }

    public static RouteGroupBuilder WithApiVersion(this RouteGroupBuilder builder, double version)
    {
        return builder.HasApiVersion(new ApiVersion(version));
    }

    public static RouteGroupBuilder WithApiVersions(this RouteGroupBuilder builder, params ApiVersion[] versions)
    {
        foreach (var version in versions)
        {
            builder.HasApiVersion(version);
        }
        return builder;
    }

    public static ApiVersionSet CreateVersionSet(this IEndpointRouteBuilder builder, string name = "default")
    {
        return builder.NewApiVersionSet(name)
            .HasApiVersion(new ApiVersion(1.0))
            .ReportApiVersions()
            .Build();
    }

    public static ApiVersionSet CreateVersionSet(this IEndpointRouteBuilder builder, string name, params double[] versions)
    {
        var versionSet = builder.NewApiVersionSet(name);
        
        foreach (var version in versions)
        {
            versionSet.HasApiVersion(new ApiVersion(version));
        }
        
        return versionSet.ReportApiVersions().Build();
    }
}