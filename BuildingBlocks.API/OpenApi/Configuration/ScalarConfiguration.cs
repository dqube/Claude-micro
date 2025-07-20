using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace BuildingBlocks.API.OpenApi.Configuration;

public static class ScalarConfiguration
{
    public static IServiceCollection AddScalarDocumentation(this IServiceCollection services)
    {
        // Scalar will be configured via UseScalar on WebApplication
        return services;
    }

    public static WebApplication UseScalarDocumentation(
        this WebApplication app,
        IConfiguration? configuration = null)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            
            app.MapScalarApiReference(options =>
            {
                options.Title = configuration?["Api:Title"] ?? "API Documentation";
                options.Theme = ScalarTheme.Purple;
                options.ShowSidebar = true;
                options.SearchHotKey = "k";
                
                // Customize the UI
                options.CustomCss = """
                    .scalar-api-reference {
                        --scalar-font-size: 14px;
                    }
                    """;
            });
        }

        return app;
    }

    public static WebApplication UseScalarDocumentation(
        this WebApplication app,
        Action<ScalarOptions> configureOptions)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(configureOptions);
        }

        return app;
    }

    public static WebApplication UseScalarDocumentation(
        this WebApplication app,
        string title,
        string? endpointPath = "/scalar",
        bool alwaysShow = false)
    {
        if (app.Environment.IsDevelopment() || alwaysShow)
        {
            app.MapOpenApi();
            
            app.MapScalarApiReference(options =>
            {
                options.Title = title;
                options.Theme = ScalarTheme.Purple;
                options.ShowSidebar = true;
                options.EndpointPathPrefix = endpointPath;
            });
        }

        return app;
    }
}