using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using BuildingBlocks.API.Utilities.Helpers;
using System.Security.Claims;

namespace BuildingBlocks.API.Endpoints.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder MapApiGet(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate handler)
    {
        return endpoints.MapGet(pattern, handler)
            .WithOpenApi()
            .Produces(200)
            .Produces(400)
            .Produces(404)
            .Produces(500);
    }

    public static RouteHandlerBuilder MapApiPost(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate handler)
    {
        return endpoints.MapPost(pattern, handler)
            .WithOpenApi()
            .Produces(201)
            .Produces(400)
            .Produces(422)
            .Produces(500);
    }

    public static RouteHandlerBuilder MapApiPut(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate handler)
    {
        return endpoints.MapPut(pattern, handler)
            .WithOpenApi()
            .Produces(200)
            .Produces(400)
            .Produces(404)
            .Produces(422)
            .Produces(500);
    }

    public static RouteHandlerBuilder MapApiDelete(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate handler)
    {
        return endpoints.MapDelete(pattern, handler)
            .WithOpenApi()
            .Produces(204)
            .Produces(400)
            .Produces(404)
            .Produces(500);
    }

    public static RouteHandlerBuilder MapApiPatch(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate handler)
    {
        return endpoints.MapPatch(pattern, handler)
            .WithOpenApi()
            .Produces(200)
            .Produces(400)
            .Produces(404)
            .Produces(422)
            .Produces(500);
    }

    public static RouteGroupBuilder MapApiGroup(
        this IEndpointRouteBuilder endpoints,
        string prefix)
    {
        return endpoints.MapGroup(prefix)
            .WithTags(GetTagFromPrefix(prefix))
            .WithOpenApi();
    }

    public static RouteGroupBuilder MapAuthenticatedApiGroup(
        this IEndpointRouteBuilder endpoints,
        string prefix)
    {
        return endpoints.MapGroup(prefix)
            .WithTags(GetTagFromPrefix(prefix))
            .WithOpenApi()
            .RequireAuthorization();
    }

    public static RouteGroupBuilder MapVersionedApiGroup(
        this IEndpointRouteBuilder endpoints,
        string prefix,
        string version)
    {
        var versionedPrefix = $"{prefix}/v{version}";
        return endpoints.MapGroup(versionedPrefix)
            .WithTags($"{GetTagFromPrefix(prefix)} v{version}")
            .WithOpenApi();
    }

    public static RouteHandlerBuilder WithApiResponse<T>(
        this RouteHandlerBuilder builder,
        int statusCode = 200,
        string? description = null)
    {
        return builder.Produces<T>(statusCode, contentType: "application/json", description);
    }

    public static RouteHandlerBuilder WithApiErrorResponses(
        this RouteHandlerBuilder builder)
    {
        return builder
            .Produces(400, description: "Bad Request")
            .Produces(401, description: "Unauthorized")
            .Produces(403, description: "Forbidden")
            .Produces(404, description: "Not Found")
            .Produces(422, description: "Validation Error")
            .Produces(429, description: "Rate Limited")
            .Produces(500, description: "Internal Server Error");
    }

    public static RouteHandlerBuilder WithCorrelationId(
        this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var correlationId = CorrelationHelper.GetOrCreateCorrelationId(context.HttpContext);
            context.HttpContext.Items["CorrelationId"] = correlationId;
            return await next(context);
        });
    }

    public static RouteHandlerBuilder WithValidation<T>(
        this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<ValidationFilter<T>>();
    }

    public static RouteHandlerBuilder WithRateLimit(
        this RouteHandlerBuilder builder,
        string policyName)
    {
        return builder.RequireRateLimiting(policyName);
    }

    public static RouteHandlerBuilder WithUserRole(
        this RouteHandlerBuilder builder,
        params string[] roles)
    {
        return builder.RequireAuthorization(policy =>
        {
            policy.RequireRole(roles);
        });
    }

    public static RouteHandlerBuilder WithUserClaim(
        this RouteHandlerBuilder builder,
        string claimType,
        params string[] claimValues)
    {
        return builder.RequireAuthorization(policy =>
        {
            policy.RequireClaim(claimType, claimValues);
        });
    }

    public static RouteHandlerBuilder WithPermission(
        this RouteHandlerBuilder builder,
        string permission)
    {
        return builder.RequireAuthorization(policy =>
        {
            policy.RequireClaim("permission", permission);
        });
    }

    private static string GetTagFromPrefix(string prefix)
    {
        return prefix.TrimStart('/').Split('/')[0]
            .Replace("-", " ")
            .ToTitleCase();
    }
}

static class StringExtensions
{
    public static string ToTitleCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return char.ToUpperInvariant(input[0]) + input[1..].ToLowerInvariant();
    }
}

public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Basic validation filter - can be extended with FluentValidation
        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        
        if (argument == null)
        {
            return Results.BadRequest("Request body is required");
        }

        return await next(context);
    }
}