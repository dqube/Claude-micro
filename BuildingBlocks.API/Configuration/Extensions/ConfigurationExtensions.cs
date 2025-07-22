using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.API.Configuration.Options;

namespace BuildingBlocks.API.Configuration.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddApiConfiguration(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<ApiOptions>(configuration.GetSection(ApiOptions.SectionName));
        services.Configure<CorsOptions>(configuration.GetSection(CorsOptions.SectionName));
        services.Configure<AuthenticationOptions>(configuration.GetSection(AuthenticationOptions.SectionName));
        services.Configure<RateLimitingOptions>(configuration.GetSection(RateLimitingOptions.SectionName));

        return services;
    }

    public static T GetRequiredSection<T>(this IConfiguration configuration, string sectionName) 
        where T : class, new()
    {
        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            throw new InvalidOperationException($"Configuration section '{sectionName}' is missing");
        }

        var options = section.Get<T>();
        if (options == null)
        {
            throw new InvalidOperationException($"Configuration section '{sectionName}' could not be bound to type {typeof(T).Name}");
        }

        return options;
    }

    public static T GetOptionalSection<T>(this IConfiguration configuration, string sectionName) 
        where T : class, new()
    {
        var section = configuration.GetSection(sectionName);
        return section.Exists() ? section.Get<T>() ?? new T() : new T();
    }

    public static bool SectionExists(this IConfiguration configuration, string sectionName)
    {
        return configuration.GetSection(sectionName).Exists();
    }

    public static string GetConnectionStringOrThrow(this IConfiguration configuration, string name)
    {
        var connectionString = configuration.GetConnectionString(name);
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{name}' is missing or empty");
        }
        return connectionString;
    }

    public static string GetRequiredValue(this IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException($"Configuration value '{key}' is missing or empty");
        }
        return value;
    }
}

public static class OptionsExtensions
{
    public static IServiceCollection ConfigureValidatedOptions<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where T : class
    {
        services.AddOptions<T>()
            .Bind(configuration.GetSection(sectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection ConfigureValidatedOptions<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName,
        Func<T, bool> validation,
        string validationErrorMessage)
        where T : class
    {
        services.AddOptions<T>()
            .Bind(configuration.GetSection(sectionName))
            .Validate(validation, validationErrorMessage)
            .ValidateOnStart();

        return services;
    }
}