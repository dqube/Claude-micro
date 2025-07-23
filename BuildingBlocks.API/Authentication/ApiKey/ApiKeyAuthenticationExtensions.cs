using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.API.Authentication.ApiKey;

public static class ApiKeyAuthenticationExtensions
{
    public static IServiceCollection AddApiKeyAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        var apiKeySettings = configuration.GetSection("Authentication:ApiKey");
        var validApiKeys = apiKeySettings.GetSection("ValidKeys").Get<string[]>() ?? Array.Empty<string>();

        services.AddAuthentication()
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationOptions.DefaultScheme,
                options =>
                {
                    options.ValidApiKeys = new HashSet<string>(validApiKeys);
                });

        return services;
    }

    public static IServiceCollection AddApiKeyAuthentication(
        this IServiceCollection services,
        params string[] validApiKeys)
    {
        services.AddAuthentication()
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationOptions.DefaultScheme,
                options =>
                {
                    options.ValidApiKeys = new HashSet<string>(validApiKeys);
                });

        return services;
    }

    public static IServiceCollection AddApiKeyAuthentication(
        this IServiceCollection services,
        Action<ApiKeyAuthenticationOptions> configureOptions)
    {
        services.AddAuthentication()
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationOptions.DefaultScheme,
                configureOptions);

        return services;
    }
}