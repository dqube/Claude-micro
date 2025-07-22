using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Scalar.AspNetCore;
using BuildingBlocks.API.OpenApi.Filters;
using BuildingBlocks.API.OpenApi.Configuration;

namespace BuildingBlocks.API.OpenApi.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiDocumentation(
        this IServiceCollection services,
        ApiDocumentationOptions options)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = options.Title,
                Version = options.Version,
                Description = options.Description,
                Contact = string.IsNullOrEmpty(options.Contact.Name) ? null : new OpenApiContact
                {
                    Name = options.Contact.Name,
                    Email = options.Contact.Email,
                    Url = string.IsNullOrEmpty(options.Contact.Url) ? null : new Uri(options.Contact.Url)
                },
                License = string.IsNullOrEmpty(options.License.Name) ? null : new OpenApiLicense
                {
                    Name = options.License.Name,
                    Url = string.IsNullOrEmpty(options.License.Url) ? null : new Uri(options.License.Url)
                }
            });

            // Add security definitions
            if (options.Security.EnableJwt)
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
            }

            if (options.Security.EnableApiKey)
            {
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "API Key Authentication",
                    Name = options.Security.ApiKeyHeaderName,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
            }

            // Add custom filters
            c.OperationFilter<AuthorizationOperationFilter>();
            c.OperationFilter<DefaultResponseOperationFilter>();

            // Include XML comments if enabled
            if (options.IncludeXmlComments)
            {
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            }
        });

        return services;
    }

    public static IServiceCollection AddScalarDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        return services;
    }
}

public static class ScalarExtensions
{
    public static WebApplication UseScalarDocumentation(
        this WebApplication app, 
        Microsoft.Extensions.Configuration.IConfiguration? configuration = null)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            
            // Use Scalar instead of SwaggerUI
            app.MapScalarApiReference();
        }

        return app;
    }
}