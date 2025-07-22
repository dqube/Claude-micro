using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Reflection;
using BuildingBlocks.API.Endpoints.Base;

namespace BuildingBlocks.API.Endpoints.Extensions;

public static class MinimalApiExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(EndpointBase)) && !t.IsAbstract)
            .ToList();

        foreach (var endpointType in endpointTypes)
        {
            if (Activator.CreateInstance(endpointType) is EndpointBase endpoint)
            {
                endpoint.MapEndpoints(app);
            }
        }

        return app;
    }

    public static WebApplication MapEndpoints(this WebApplication app, Assembly assembly)
    {
        var endpointTypes = assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(EndpointBase)) && !t.IsAbstract)
            .ToList();

        foreach (var endpointType in endpointTypes)
        {
            if (Activator.CreateInstance(endpointType) is EndpointBase endpoint)
            {
                endpoint.MapEndpoints(app);
            }
        }

        return app;
    }

    public static WebApplication MapEndpoints(this WebApplication app, params Type[] endpointTypes)
    {
        foreach (var endpointType in endpointTypes)
        {
            if (Activator.CreateInstance(endpointType) is EndpointBase endpoint)
            {
                endpoint.MapEndpoints(app);
            }
        }

        return app;
    }

    public static WebApplication MapEndpointsFromAssemblyContaining<T>(this WebApplication app)
    {
        return app.MapEndpoints(typeof(T).Assembly);
    }

    public static RouteGroupBuilder WithValidation<T>(this RouteGroupBuilder group)
    {
        group.AddEndpointFilter<ValidationFilter<T>>();
        return group;
    }

    public static RouteGroupBuilder WithCorrelationId(this RouteGroupBuilder group)
    {
        group.AddEndpointFilter(async (context, next) =>
        {
            var correlationId = BuildingBlocks.API.Utilities.Helpers.CorrelationHelper.GetOrCreateCorrelationId(context.HttpContext);
            context.HttpContext.Items["CorrelationId"] = correlationId;
            return await next(context);
        });
        return group;
    }

    public static RouteGroupBuilder WithExceptionHandling(this RouteGroupBuilder group)
    {
        group.AddEndpointFilter(async (context, next) =>
        {
            try
            {
                return await next(context);
            }
            catch (Exception ex)
            {
                var logger = context.HttpContext.RequestServices.GetService<ILogger<RouteGroupBuilder>>();
                logger?.LogError(ex, "An error occurred while processing the request");
                
                return BuildingBlocks.API.Utilities.Factories.ErrorFactory.CreateErrorResult(ex, context.HttpContext);
            }
        });
        return group;
    }

    public static RouteGroupBuilder WithRequestLogging(this RouteGroupBuilder group)
    {
        group.AddEndpointFilter(async (context, next) =>
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<RouteGroupBuilder>>();
            var httpContext = context.HttpContext;
            
            logger?.LogInformation(
                "Processing {Method} request to {Path} from {RemoteIp}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.Connection.RemoteIpAddress);

            var result = await next(context);

            logger?.LogInformation(
                "Completed {Method} request to {Path} with status {StatusCode}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.Response.StatusCode);

            return result;
        });
        return group;
    }

    public static RouteGroupBuilder WithApiDefaults(this RouteGroupBuilder group)
    {
        return group
            .WithCorrelationId()
            .WithExceptionHandling()
            .WithRequestLogging()
            .WithOpenApi();
    }

    public static RouteHandlerBuilder WithProblemDetails(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var result = await next(context);
            
            // Convert exceptions to problem details if needed
            if (result is Exception ex)
            {
                return BuildingBlocks.API.Utilities.Factories.ErrorFactory.CreateProblemResult(ex, context.HttpContext);
            }
            
            return result;
        });
    }

    public static RouteHandlerBuilder WithStandardResponses(this RouteHandlerBuilder builder)
    {
        return builder
            .Produces(200, description: "Success")
            .Produces(400, description: "Bad Request")
            .Produces(401, description: "Unauthorized")
            .Produces(403, description: "Forbidden")
            .Produces(404, description: "Not Found")
            .Produces(422, description: "Validation Error")
            .Produces(500, description: "Internal Server Error");
    }

    public static RouteHandlerBuilder WithCaching(this RouteHandlerBuilder builder, int seconds = 300)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            context.HttpContext.Response.Headers.CacheControl = $"public, max-age={seconds}";
            return await next(context);
        });
    }

    public static RouteHandlerBuilder WithNoCache(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            context.HttpContext.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
            context.HttpContext.Response.Headers.Pragma = "no-cache";
            context.HttpContext.Response.Headers.Expires = "0";
            return await next(context);
        });
    }
}