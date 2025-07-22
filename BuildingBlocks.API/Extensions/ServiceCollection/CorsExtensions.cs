using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.API.Configuration.Options;

namespace BuildingBlocks.API.Extensions.ServiceCollection;

public static class CorsExtensions
{
    public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
    {
        var corsOptions = configuration.GetSection(CorsOptions.ConfigurationSection).Get<CorsOptions>();
        
        if (corsOptions == null)
        {
            return services.AddDefaultApiCors();
        }

        return services.AddApiCors(corsOptions);
    }

    public static IServiceCollection AddApiCors(this IServiceCollection services, CorsOptions corsOptions)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(corsOptions.PolicyName, policy =>
            {
                // Configure origins
                if (corsOptions.AllowedOrigins.Length == 1 && corsOptions.AllowedOrigins[0] == "*")
                {
                    policy.AllowAnyOrigin();
                }
                else if (corsOptions.AllowedOrigins.Length > 0)
                {
                    policy.WithOrigins(corsOptions.AllowedOrigins);
                }

                // Configure methods
                if (corsOptions.AllowedMethods.Length == 1 && corsOptions.AllowedMethods[0] == "*")
                {
                    policy.AllowAnyMethod();
                }
                else if (corsOptions.AllowedMethods.Length > 0)
                {
                    policy.WithMethods(corsOptions.AllowedMethods);
                }

                // Configure headers
                if (corsOptions.AllowedHeaders.Length == 1 && corsOptions.AllowedHeaders[0] == "*")
                {
                    policy.AllowAnyHeader();
                }
                else if (corsOptions.AllowedHeaders.Length > 0)
                {
                    policy.WithHeaders(corsOptions.AllowedHeaders);
                }

                // Configure exposed headers
                if (corsOptions.ExposedHeaders.Length > 0)
                {
                    policy.WithExposedHeaders(corsOptions.ExposedHeaders);
                }

                // Configure credentials
                if (corsOptions.AllowCredentials)
                {
                    policy.AllowCredentials();
                }

                // Configure preflight max age
                if (corsOptions.PreflightMaxAge > 0)
                {
                    policy.SetPreflightMaxAge(TimeSpan.FromSeconds(corsOptions.PreflightMaxAge));
                }
            });
        });

        return services;
    }

    public static IServiceCollection AddDefaultApiCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCorsPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return services;
    }

    public static IServiceCollection AddRestrictiveCors(this IServiceCollection services, string[] allowedOrigins)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("RestrictiveCorsPolicy", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .WithMethods("GET", "POST", "PUT", "DELETE")
                      .WithHeaders("Content-Type", "Authorization", "X-API-Key", "X-Correlation-ID")
                      .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddDevelopmentCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DevelopmentCorsPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .WithExposedHeaders("X-Correlation-ID", "X-Total-Count", "X-Page-Count");
            });
        });

        return services;
    }

    public static IServiceCollection AddProductionCors(this IServiceCollection services, string[] allowedOrigins)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("ProductionCorsPolicy", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
                      .WithHeaders("Content-Type", "Authorization", "X-API-Key", "X-Correlation-ID", "Accept")
                      .WithExposedHeaders("X-Correlation-ID", "X-Total-Count", "X-Page-Count", "X-RateLimit-Limit", "X-RateLimit-Remaining")
                      .AllowCredentials()
                      .SetPreflightMaxAge(TimeSpan.FromHours(1));
            });
        });

        return services;
    }
}