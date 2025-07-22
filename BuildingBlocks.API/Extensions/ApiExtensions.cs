using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BuildingBlocks.API.Authentication.JWT;
using BuildingBlocks.API.Authentication.ApiKey;
using BuildingBlocks.API.Configuration.Extensions;
using BuildingBlocks.API.Health.Extensions;
using BuildingBlocks.API.OpenApi.Configuration;
using BuildingBlocks.API.Versioning.Extensions;
using Scalar.AspNetCore;

namespace BuildingBlocks.API.Extensions;

/// <summary>
/// Main extension methods for registering and configuring BuildingBlocks.API services
/// </summary>
public static class ApiExtensions
{
    /// <summary>
    /// Registers all BuildingBlocks.API services with dependency injection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddBuildingBlocksApi(this IServiceCollection services, IConfiguration configuration)
    {
        // Core API configuration
        services.AddApiConfiguration(configuration);
        
        // Middleware pipeline components
        services.AddApiMiddleware();
        
        // Error handling
        services.AddApiErrorHandling();
        
        // Security middleware
        services.AddApiSecurity();
        
        // Request validation
        services.AddApiValidation();
        
        // Rate limiting
        services.AddApiRateLimiting(configuration);
        
        // API versioning
        services.AddApiVersioning();
        
        // Health checks
        services.AddApiHealthChecks();
        
        // Authentication (conditional based on configuration)
        var authSection = configuration.GetSection("Authentication");
        if (authSection.GetSection("Jwt").Exists())
        {
            services.AddJwtAuthentication(configuration);
        }
        if (authSection.GetSection("ApiKey").Exists())
        {
            services.AddApiKeyAuthentication(configuration);
        }
        
        // OpenAPI documentation
        services.AddScalarApiReference();
        
        return services;
    }
    
    /// <summary>
    /// Registers BuildingBlocks.API services with custom configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <param name="configureOptions">Custom configuration action</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddBuildingBlocksApi(
        this IServiceCollection services, 
        IConfiguration configuration,
        Action<ApiRegistrationOptions> configureOptions)
    {
        var options = new ApiRegistrationOptions();
        configureOptions(options);
        
        // Core API configuration
        services.AddApiConfiguration(configuration);
        
        // Conditionally add components based on options
        if (options.IncludeMiddleware)
        {
            services.AddApiMiddleware();
        }
        
        if (options.IncludeErrorHandling)
        {
            services.AddApiErrorHandling();
        }
        
        if (options.IncludeSecurity)
        {
            services.AddApiSecurity();
        }
        
        if (options.IncludeValidation)
        {
            services.AddApiValidation();
        }
        
        if (options.IncludeRateLimiting)
        {
            services.AddApiRateLimiting(configuration);
        }
        
        if (options.IncludeVersioning)
        {
            services.AddApiVersioning();
        }
        
        if (options.IncludeHealthChecks)
        {
            services.AddApiHealthChecks();
        }
        
        if (options.IncludeAuthentication)
        {
            var authSection = configuration.GetSection("Authentication");
            if (authSection.GetSection("Jwt").Exists())
            {
                services.AddJwtAuthentication(configuration);
            }
            if (authSection.GetSection("ApiKey").Exists())
            {
                services.AddApiKeyAuthentication(configuration);
            }
        }
        
        if (options.IncludeDocumentation)
        {
            services.AddScalarApiReference();
        }
        
        return services;
    }
    
    /// <summary>
    /// Configures the BuildingBlocks.API middleware pipeline
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The web application for method chaining</returns>
    public static WebApplication UseBuildingBlocksApi(this WebApplication app, IConfiguration configuration)
    {
        // Development-specific middleware
        if (app.Environment.IsDevelopment())
        {
            app.UseDevelopmentErrorHandling();
        }
        else
        {
            app.UseApiErrorHandling();
        }
        
        // Security headers
        app.UseApiSecurityHeaders();
        
        // CORS
        app.UseCors();
        
        // Rate limiting
        app.UseRateLimiter();
        
        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();
        
        // Request correlation and logging
        app.UseCorrelationId();
        app.UseRequestLogging();
        
        // API versioning
        app.UseApiVersioning();
        
        // Health checks
        app.MapHealthChecks("/health");
        
        // OpenAPI documentation
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
        
        return app;
    }
    
    /// <summary>
    /// Configures the BuildingBlocks.API middleware pipeline with custom options
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="_">Application configuration (unused)</param>
    /// <param name="configureOptions">Custom configuration action</param>
    /// <returns>The web application for method chaining</returns>
    public static WebApplication UseBuildingBlocksApi(
        this WebApplication app, 
        IConfiguration _,
        Action<ApiPipelineOptions> configureOptions)
    {
        var options = new ApiPipelineOptions();
        configureOptions(options);
        
        // Development-specific middleware
        if (app.Environment.IsDevelopment() && options.IncludeDevelopmentErrorHandling)
        {
            app.UseDevelopmentErrorHandling();
        }
        else if (options.IncludeErrorHandling)
        {
            app.UseApiErrorHandling();
        }
        
        // Security headers
        if (options.IncludeSecurityHeaders)
        {
            app.UseApiSecurityHeaders();
        }
        
        // CORS
        if (options.IncludeCors)
        {
            app.UseCors();
        }
        
        // Rate limiting
        if (options.IncludeRateLimiting)
        {
            app.UseRateLimiter();
        }
        
        // Authentication & Authorization
        if (options.IncludeAuthentication)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
        
        // Request correlation and logging
        if (options.IncludeRequestLogging)
        {
            app.UseCorrelationId();
            app.UseRequestLogging();
        }
        
        // API versioning
        if (options.IncludeVersioning)
        {
            app.UseApiVersioning();
        }
        
        // Health checks
        if (options.IncludeHealthChecks)
        {
            app.MapHealthChecks(options.HealthCheckPath);
        }
        
        // OpenAPI documentation
        if (options.IncludeDocumentation && (app.Environment.IsDevelopment() || options.EnableDocumentationInProduction))
        {
            app.MapOpenApi();
            app.MapScalarApiReference(opts => opts.WithTitle(options.DocumentationTitle));
        }
        
        return app;
    }
}

/// <summary>
/// Options for configuring API service registration
/// </summary>
public class ApiRegistrationOptions
{
    public bool IncludeMiddleware { get; set; } = true;
    public bool IncludeErrorHandling { get; set; } = true;
    public bool IncludeSecurity { get; set; } = true;
    public bool IncludeValidation { get; set; } = true;
    public bool IncludeRateLimiting { get; set; } = true;
    public bool IncludeVersioning { get; set; } = true;
    public bool IncludeHealthChecks { get; set; } = true;
    public bool IncludeAuthentication { get; set; } = true;
    public bool IncludeDocumentation { get; set; } = true;
}

/// <summary>
/// Options for configuring API middleware pipeline
/// </summary>
public class ApiPipelineOptions
{
    public bool IncludeErrorHandling { get; set; } = true;
    public bool IncludeDevelopmentErrorHandling { get; set; } = true;
    public bool IncludeSecurityHeaders { get; set; } = true;
    public bool IncludeCors { get; set; } = true;
    public bool IncludeRateLimiting { get; set; } = true;
    public bool IncludeAuthentication { get; set; } = true;
    public bool IncludeRequestLogging { get; set; } = true;
    public bool IncludeVersioning { get; set; } = true;
    public bool IncludeHealthChecks { get; set; } = true;
    public bool IncludeDocumentation { get; set; } = true;
    public bool EnableDocumentationInProduction { get; set; } = false;
    public string HealthCheckPath { get; set; } = "/health";
    public string DocumentationTitle { get; set; } = "API Documentation";
}