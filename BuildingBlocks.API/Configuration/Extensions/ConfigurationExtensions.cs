using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using BuildingBlocks.API.Configuration.Options;

namespace BuildingBlocks.API.Configuration.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddApiOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ApiOptions>()
            .Bind(configuration.GetSection(ApiOptions.ConfigurationSection))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<CorsOptions>()
            .Bind(configuration.GetSection(CorsOptions.ConfigurationSection))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<AuthenticationOptions>()
            .Bind(configuration.GetSection(AuthenticationOptions.ConfigurationSection))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<RateLimitingOptions>()
            .Bind(configuration.GetSection(RateLimitingOptions.ConfigurationSection))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public static T GetRequiredOptions<T>(this IConfiguration configuration, string sectionName)
        where T : class, new()
    {
        var options = new T();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }

    public static bool TryGetOptions<T>(this IConfiguration configuration, string sectionName, out T? options)
        where T : class, new()
    {
        options = new T();
        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            options = null;
            return false;
        }

        section.Bind(options);
        return true;
    }
}