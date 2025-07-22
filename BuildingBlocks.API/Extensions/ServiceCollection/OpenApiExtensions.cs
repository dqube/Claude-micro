using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerUI;
using BuildingBlocks.API.Configuration.Options;
using BuildingBlocks.API.OpenApi.Configuration;

namespace BuildingBlocks.API.Extensions.ServiceCollection;

public static class OpenApiExtensions
{
    public static Microsoft.AspNetCore.Builder.WebApplication UseApiDocumentation(this Microsoft.AspNetCore.Builder.WebApplication app)
    {
        var apiOptions = app.Services.GetService<IOptions<ApiOptions>>()?.Value;
        
        if (apiOptions == null)
        {
            return app;
        }

        return app.UseApiDocumentation(apiOptions);
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseApiDocumentation(this Microsoft.AspNetCore.Builder.WebApplication app, ApiOptions apiOptions)
    {
        // Use Swagger UI
        if (apiOptions.EnableSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{apiOptions.Title} v{apiOptions.Version}");
                options.RoutePrefix = "swagger";
                options.DocumentTitle = $"{apiOptions.Title} API Documentation";
                options.DisplayRequestDuration();
                options.EnableTryItOutByDefault();
            });
        }

        // Use Scalar if enabled
        if (apiOptions.EnableScalar)
        {
            app.UseScalar(apiOptions);
        }

        return app;
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseScalar(this Microsoft.AspNetCore.Builder.WebApplication app, ApiOptions apiOptions)
    {
        var scalarConfig = app.Services.GetService<IOptions<ScalarOptions>>()?.Value ?? new ScalarOptions
        {
            Title = apiOptions.Title,
            Description = apiOptions.Description,
            Version = apiOptions.Version
        };

        app.MapScalarApiReference(options =>
        {
            options.WithTitle(scalarConfig.Title)
                   .WithDescription(scalarConfig.Description)
                   .WithVersion(scalarConfig.Version)
                   .WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json")
                   .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });

        return app;
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseSwaggerDocumentation(this Microsoft.AspNetCore.Builder.WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            options.RoutePrefix = "swagger";
        });
        
        return app;
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseVersionedSwagger(this Microsoft.AspNetCore.Builder.WebApplication app, params string[] versions)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var version in versions)
            {
                options.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"API {version.ToUpper()}");
            }
            options.RoutePrefix = "swagger";
        });
        
        return app;
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseReDoc(this Microsoft.AspNetCore.Builder.WebApplication app)
    {
        app.UseReDoc(options =>
        {
            options.SpecUrl = "/swagger/v1/swagger.json";
            options.RoutePrefix = "redoc";
        });
        
        return app;
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseDevelopmentDocumentation(this Microsoft.AspNetCore.Builder.WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseApiDocumentation();
        }
        
        return app;
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseProductionDocumentation(this Microsoft.AspNetCore.Builder.WebApplication app)
    {
        // Only enable in production if explicitly configured
        var apiOptions = app.Services.GetService<IOptions<ApiOptions>>()?.Value;
        
        if (apiOptions?.EnableSwagger == true || apiOptions?.EnableScalar == true)
        {
            app.UseApiDocumentation(apiOptions);
        }
        
        return app;
    }

    public static Microsoft.AspNetCore.Builder.WebApplication UseCustomDocumentation(this Microsoft.AspNetCore.Builder.WebApplication app, Action<SwaggerUIOptions> configureSwagger, Action<ScalarOptions> configureScalar)
    {
        app.UseSwagger();
        app.UseSwaggerUI(configureSwagger);
        app.MapScalarApiReference(configureScalar);
        
        return app;
    }
}

// Extension method for Scalar (placeholder - actual implementation would depend on Scalar package)
public static class ScalarExtensions
{
    public static Microsoft.AspNetCore.Builder.WebApplication MapScalarApiReference(this Microsoft.AspNetCore.Builder.WebApplication app, Action<ScalarOptions> configure)
    {
        var options = new ScalarOptions();
        configure(options);
        
        // This would be replaced with actual Scalar implementation
        app.MapGet("/scalar", () => Results.Redirect("/swagger"));
        
        return app;
    }
}

public class ScalarOptions
{
    public string Title { get; private set; } = "API Documentation";
    public string Description { get; private set; } = "";
    public string Version { get; private set; } = "1.0";
    public string OpenApiRoutePattern { get; private set; } = "/swagger/v1/swagger.json";
    
    public ScalarOptions WithTitle(string title)
    {
        Title = title;
        return this;
    }
    
    public ScalarOptions WithDescription(string description)
    {
        Description = description;
        return this;
    }
    
    public ScalarOptions WithVersion(string version)
    {
        Version = version;
        return this;
    }
    
    public ScalarOptions WithOpenApiRoutePattern(string pattern)
    {
        OpenApiRoutePattern = pattern;
        return this;
    }
    
    public ScalarOptions WithDefaultHttpClient(ScalarTarget target, ScalarClient client)
    {
        return this;
    }
}

public enum ScalarTarget
{
    CSharp,
    JavaScript,
    Python
}

public enum ScalarClient
{
    HttpClient,
    Axios,
    Fetch
}