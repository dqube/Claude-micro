using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using BuildingBlocks.API.Authentication.JWT;
using BuildingBlocks.API.Authentication.ApiKey;
using BuildingBlocks.API.Configuration.Options;
using ApiAuthOptions = BuildingBlocks.API.Configuration.Options.AuthenticationOptions;

namespace BuildingBlocks.API.Extensions.ServiceCollection;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authOptions = configuration.GetSection(ApiAuthOptions.ConfigurationSection).Get<ApiAuthOptions>();
        
        if (authOptions == null)
        {
            throw new InvalidOperationException("Authentication configuration is missing");
        }

        return services.AddApiAuthentication(authOptions);
    }

    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, ApiAuthOptions authOptions)
    {
        var authBuilder = services.AddAuthentication();

        // Configure JWT if enabled
        if (!string.IsNullOrEmpty(authOptions.Jwt.Issuer) || !string.IsNullOrEmpty(authOptions.Jwt.Authority))
        {
            authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => { });
            services.ConfigureOptions<JwtBearerOptionsSetup>();
        }

        // Configure API Key if enabled
        if (authOptions.ApiKey.Enabled)
        {
            authBuilder.AddApiKeyAuthentication();
        }

        // Set default scheme
        if (!string.IsNullOrEmpty(authOptions.Jwt.Issuer) || !string.IsNullOrEmpty(authOptions.Jwt.Authority))
        {
            authBuilder.Services.Configure<ApiAuthOptions>(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
        }

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => { });
            
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        
        return services;
    }

    public static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authOptions = configuration.GetSection(ApiAuthOptions.ConfigurationSection).Get<ApiAuthOptions>();
        
        if (authOptions?.ApiKey.Enabled == true)
        {
            services.AddAuthentication()
                .AddApiKeyAuthentication();
        }
        
        return services;
    }

    public static AuthenticationBuilder AddApiKeyAuthentication(this AuthenticationBuilder builder)
    {
        return builder.AddScheme<ApiKeyAuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
            "ApiKey", 
            "API Key Authentication", 
            options => { });
    }

    public static IServiceCollection AddApiAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddDefaultPolicy("DefaultAuthPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
            })
            .AddPolicy("ApiKeyPolicy", policy =>
            {
                policy.AddAuthenticationSchemes("ApiKey");
                policy.RequireAuthenticatedUser();
            })
            .AddPolicy("JwtPolicy", policy =>
            {
                policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            });

        return services;
    }
}

public class ApiKeyAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public string HeaderName { get; set; } = "X-API-Key";
    public string QueryParameterName { get; set; } = "apiKey";
}