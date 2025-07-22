using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using BuildingBlocks.API.OpenApi.Configuration;
using BuildingBlocks.API.Configuration.Options;

namespace BuildingBlocks.API.OpenApi.Extensions;

public static class ScalarExtensions
{
    public static IServiceCollection AddScalarDocumentation(this IServiceCollection services, ApiOptions apiOptions)
    {
        services.Configure<ScalarOptions>(options =>
        {
            options.Title = apiOptions.Title;
            options.Description = apiOptions.Description;
            options.Version = apiOptions.Version;
        });

        return services;
    }

    public static IServiceCollection AddScalarDocumentation(this IServiceCollection services, Action<ScalarOptions> configure)
    {
        services.Configure(configure);
        return services;
    }

    public static IServiceCollection ConfigureScalar(this IServiceCollection services, ScalarOptions configuration)
    {
        services.Configure<ScalarOptions>(options =>
        {
            options.Title = configuration.Title;
            options.Description = configuration.Description;
            options.Version = configuration.Version;
            options.OpenApiUrl = configuration.OpenApiUrl;
            options.DarkMode = configuration.DarkMode;
            options.ShowSidebar = configuration.ShowSidebar;
            options.HideModels = configuration.HideModels;
            options.CustomCss = configuration.CustomCss;
            options.FaviconUrl = configuration.FaviconUrl;
            options.LogoUrl = configuration.LogoUrl;
        });

        return services;
    }

    public static ScalarOptions WithTheme(this ScalarOptions config, bool darkMode = false, string? customCss = null)
    {
        config.DarkMode = darkMode;
        config.CustomCss = customCss;
        return config;
    }

    public static ScalarOptions WithBranding(this ScalarOptions config, string? logoUrl = null, string? faviconUrl = null)
    {
        config.LogoUrl = logoUrl;
        config.FaviconUrl = faviconUrl;
        return config;
    }

    public static ScalarOptions WithLayout(this ScalarOptions config, bool showSidebar = true, bool hideModels = false)
    {
        config.ShowSidebar = showSidebar;
        config.HideModels = hideModels;
        return config;
    }

    public static ScalarOptions WithOpenApiUrl(this ScalarOptions config, string openApiUrl)
    {
        config.OpenApiUrl = openApiUrl;
        return config;
    }

    public static ScalarOptions CreateDefault(string title, string version = "1.0", string description = "")
    {
        return new ScalarOptions
        {
            Title = title,
            Version = version,
            Description = description,
            OpenApiUrl = "/swagger/v1/swagger.json",
            DarkMode = false,
            ShowSidebar = true,
            HideModels = false
        };
    }

    public static ScalarOptions CreateDarkTheme(string title, string version = "1.0", string description = "")
    {
        return CreateDefault(title, version, description)
            .WithTheme(darkMode: true);
    }

    public static ScalarOptions CreateMinimal(string title, string version = "1.0", string description = "")
    {
        return CreateDefault(title, version, description)
            .WithLayout(showSidebar: false, hideModels: true);
    }

    public static ScalarOptions CreateBranded(
        string title, 
        string version = "1.0", 
        string description = "",
        string? logoUrl = null,
        string? faviconUrl = null)
    {
        return CreateDefault(title, version, description)
            .WithBranding(logoUrl, faviconUrl);
    }
}