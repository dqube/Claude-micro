using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;

namespace BuildingBlocks.API.OpenApi.Configuration;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddOpenApiDocumentation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var apiSettings = configuration.GetSection("Api");
        var title = apiSettings["Title"] ?? "API";
        var version = apiSettings["Version"] ?? "v1";
        var description = apiSettings["Description"] ?? "API Documentation";

        services.AddOpenApi();

        services.AddEndpointsApiExplorer();

        return services;
    }

    public static IServiceCollection AddOpenApiDocumentation(
        this IServiceCollection services,
        string title,
        string version,
        string? description = null)
    {
        services.AddOpenApi();

        services.AddEndpointsApiExplorer();

        return services;
    }

    public static IServiceCollection AddOpenApiWithAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        bool includeJwtBearer = true,
        bool includeApiKey = true)
    {
        services.AddOpenApiDocumentation(configuration);

        return services;
    }
}