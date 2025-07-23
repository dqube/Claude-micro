using BuildingBlocks.API.Authentication.ApiKey;
using BuildingBlocks.API.Authentication.JWT;
using BuildingBlocks.API.Configuration.Extensions;
using BuildingBlocks.API.Configuration.Options;
using BuildingBlocks.API.Health.Extensions;
using BuildingBlocks.API.OpenApi.Extensions;
using BuildingBlocks.API.Versioning.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        
        // CORS
        services.AddApiCors(configuration);
        
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
        
        // Authorization services (required for UseAuthorization middleware)
        services.AddAuthorization();
        
        // OpenAPI documentation
        services.AddOpenApiDocumentation(configuration);
        
        // JSON configuration with custom converters
        services.AddApiJsonConfiguration();
        
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
        ArgumentNullException.ThrowIfNull(configureOptions);
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
        
        if (options.IncludeCors)
        {
            services.AddApiCors(configuration);
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
            
            // Authorization services (required for UseAuthorization middleware)
            services.AddAuthorization();
        }
        
        if (options.IncludeDocumentation)
        {
            services.AddOpenApiDocumentation(configuration);
        }
        
        // JSON configuration with custom converters (always included)
        services.AddApiJsonConfiguration();
        
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
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configuration);
        app.UseOpenApiDocumentation();

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
      // app.UseApiSecurityHeaders();
        
        // CORS
       app.UseCors();

        // Rate limiting (conditional based on configuration)
        var rateLimitSection = configuration.GetSection("RateLimiting");
        if (rateLimitSection.Exists() && rateLimitSection.GetValue<bool>("Enabled", true))
        {
            app.UseRateLimiter();
        }

        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        //Request correlation and logging
        app.UseCorrelationId();
        app.UseRequestLogging();

        //API versioning
         app.UseApiVersioning();

        //Health checks
         app.MapHealthChecks("/health");



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
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configureOptions);
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
            app.MapOpenApi("/openapi/{documentName}.json");
            app.MapScalarApiReference(opts => opts.WithTitle(options.DocumentationTitle));
        }
        
        return app;
    }
    
    /// <summary>
    /// Adds CORS services to the service collection with configuration from appsettings
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        var corsOptions = configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>() ?? new CorsOptions();
        
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                // Configure allowed origins
                if (corsOptions.AllowedOrigins.Length > 0)
                {
                    policy.WithOrigins(corsOptions.AllowedOrigins);
                }
                else
                {
                    policy.AllowAnyOrigin();
                }
                
                // Configure allowed methods
                if (corsOptions.AllowedMethods.Length > 0)
                {
                    policy.WithMethods(corsOptions.AllowedMethods);
                }
                else
                {
                    policy.AllowAnyMethod();
                }
                
                // Configure allowed headers
                if (corsOptions.AllowedHeaders.Length > 0)
                {
                    policy.WithHeaders(corsOptions.AllowedHeaders);
                }
                else
                {
                    policy.AllowAnyHeader();
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
                policy.SetPreflightMaxAge(TimeSpan.FromSeconds(corsOptions.PreflightMaxAge));
            });
        });
        
        return services;
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
    public bool IncludeCors { get; set; } = true;
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