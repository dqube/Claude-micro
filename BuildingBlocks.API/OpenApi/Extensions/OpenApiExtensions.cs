using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using BuildingBlocks.API.OpenApi.Configuration;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.API.OpenApi.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiDocumentation(
        this IServiceCollection services,IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        
        var apiSettings = configuration.GetSection("Api");
        var title = apiSettings["Title"] ?? "API";
        var version = apiSettings["Version"] ?? "v1";
        var description = apiSettings["Description"] ?? "API Documentation";

        services.AddOpenApi("v1");
        services.AddEndpointsApiExplorer();
        return services;
    }
   
}

public static class ScalarExtensions
{
    public static WebApplication UseOpenApiDocumentation(
        this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            // Use Scalar instead of SwaggerUI
            app.MapScalarApiReference();
        }

        return app;
    }
}