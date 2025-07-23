using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.API.Endpoints.Extensions;

public static class MinimalApiExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);
        
        var endpointTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(i => i == typeof(IEndpoint)))
            .ToList();

        foreach (var endpointType in endpointTypes)
        {
            services.AddScoped(endpointType);
        }

        return services;
    }

    public static WebApplication MapEndpoints(this WebApplication app, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(assembly);
        
        var endpointTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(i => i == typeof(IEndpoint)))
            .ToList();

        using var scope = app.Services.CreateScope();

        foreach (var endpointType in endpointTypes)
        {
            if (scope.ServiceProvider.GetService(endpointType) is IEndpoint endpoint)
            {
                endpoint.MapEndpoints(app);
            }
        }

        return app;
    }

    public static RouteGroupBuilder WithApiTags(this RouteGroupBuilder group, params string[] tags)
    {
        return group.WithTags(tags);
    }

    public static RouteHandlerBuilder WithApiSummary(this RouteHandlerBuilder builder, string summary, string? description = null)
    {
        builder.WithSummary(summary);
        if (!string.IsNullOrEmpty(description))
        {
            builder.WithDescription(description);
        }
        return builder;
    }

    public static RouteHandlerBuilder ProducesApiResponse<T>(this RouteHandlerBuilder builder, int statusCode = 200)
    {
        return builder.Produces<T>(statusCode);
    }

    public static RouteHandlerBuilder ProducesApiError(this RouteHandlerBuilder builder, int statusCode = 400)
    {
        return builder.ProducesProblem(statusCode);
    }
}

public interface IEndpoint
{
    void MapEndpoints(IEndpointRouteBuilder endpoints);
}