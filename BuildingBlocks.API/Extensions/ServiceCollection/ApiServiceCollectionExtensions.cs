using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using BuildingBlocks.API.Authentication.JWT;
using BuildingBlocks.API.Authentication.ApiKey;
using BuildingBlocks.API.OpenApi.Configuration;
using BuildingBlocks.API.Validation.Extensions;

namespace BuildingBlocks.API.Extensions.ServiceCollection;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddBuildingBlocksApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add core API services
        services.AddApiCore();
        
        // Add authentication
        services.AddApiAuthentication(configuration);
        
        // Add OpenAPI documentation
        services.AddOpenApiWithAuthentication(configuration);
        
        // Add validation
        services.AddFluentValidation();
        
        // Add CORS
        services.AddApiCors(configuration);
        
        // Add rate limiting (commented out due to package availability)
        // services.AddApiRateLimiting(configuration);
        
        // Add health checks
        services.AddApiHealthChecks();

        return services;
    }

    public static IServiceCollection AddApiCore(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddProblemDetails();
        
        // Configure JSON options
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.SerializerOptions.WriteIndented = true;
        });

        return services;
    }

    public static IServiceCollection AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authSection = configuration.GetSection("Authentication");
        
        // Add JWT if configured
        if (authSection.GetSection("Jwt").Exists())
        {
            services.AddJwtAuthentication(configuration);
        }
        
        // Add API Key if configured
        if (authSection.GetSection("ApiKey").Exists())
        {
            services.AddApiKeyAuthentication(configuration);
        }

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddApiCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var corsSettings = configuration.GetSection("Cors");
        
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                var origins = corsSettings.GetSection("AllowedOrigins").Get<string[]>();
                var methods = corsSettings.GetSection("AllowedMethods").Get<string[]>();
                var headers = corsSettings.GetSection("AllowedHeaders").Get<string[]>();

                if (origins?.Any() == true)
                {
                    builder.WithOrigins(origins);
                }
                else
                {
                    builder.AllowAnyOrigin();
                }

                if (methods?.Any() == true)
                {
                    builder.WithMethods(methods);
                }
                else
                {
                    builder.AllowAnyMethod();
                }

                if (headers?.Any() == true)
                {
                    builder.WithHeaders(headers);
                }
                else
                {
                    builder.AllowAnyHeader();
                }
            });
        });

        return services;
    }

    // Commented out due to package availability in .NET 9
    // public static IServiceCollection AddApiRateLimiting(
    //     this IServiceCollection services,
    //     IConfiguration configuration)
    // {
    //     // Rate limiting implementation would go here
    //     // Can be implemented with custom middleware or third-party packages
    //     return services;
    // }

    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

        return services;
    }
}