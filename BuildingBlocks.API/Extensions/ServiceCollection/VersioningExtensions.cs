using Microsoft.Extensions.DependencyInjection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using BuildingBlocks.API.Versioning.Conventions;

namespace BuildingBlocks.API.Extensions.ServiceCollection;

public static class VersioningExtensions
{
    public static IServiceCollection AddApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            VersioningConvention.ConfigureDefaultVersioning(options);
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

    public static IServiceCollection AddDefaultApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            VersioningConvention.ConfigureDefaultVersioning(options);
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
            options.DefaultApiVersion = new ApiVersion(1.0);
        });

        return services;
    }

    public static IServiceCollection AddConservativeApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            VersioningConvention.ConfigureConservativeVersioning(options);
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    public static IServiceCollection AddFlexibleApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            VersioningConvention.ConfigureFlexibleVersioning(options);
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
        });

        return services;
    }

    public static IServiceCollection AddUrlSegmentVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1.0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    public static IServiceCollection AddHeaderVersioning(this IServiceCollection services, string headerName = "X-Version")
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1.0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new HeaderApiVersionReader(headerName);
        })
        .AddApiExplorer();

        return services;
    }

    public static IServiceCollection AddQueryStringVersioning(this IServiceCollection services, string parameterName = "version")
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1.0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new QueryStringApiVersionReader(parameterName);
        })
        .AddApiExplorer();

        return services;
    }
}