using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.API.Endpoints.Extensions;

/// <summary>
/// Extension methods for IEndpointRouteBuilder to simplify endpoint configuration
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps a GET endpoint with consistent configuration
    /// </summary>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="handler">The request handler</param>
    /// <param name="tags">OpenAPI tags</param>
    /// <param name="summary">OpenAPI summary</param>
    /// <param name="requireAuth">Whether authentication is required</param>
    /// <returns>Route handler builder</returns>
    public static RouteHandlerBuilder MapApiGet(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate handler,
        string[]? tags = null,
        string? summary = null,
        bool requireAuth = false)
    {
        var builder = endpoints.MapGet(pattern, handler);

        if (tags != null && tags.Length > 0)
            builder.WithTags(tags);

        if (!string.IsNullOrEmpty(summary))
            builder.WithSummary(summary);

        if (requireAuth)
            builder.RequireAuthorization();

        return builder.WithOpenApi();
    }

    /// <summary>
    /// Maps a POST endpoint with consistent configuration
    /// </summary>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="handler">The request handler</param>
    /// <param name="tags">OpenAPI tags</param>
    /// <param name="summary">OpenAPI summary</param>
    /// <param name="requireAuth">Whether authentication is required</param>
    /// <returns>Route handler builder</returns>
    public static RouteHandlerBuilder MapApiPost(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate handler,
        string[]? tags = null,
        string? summary = null,
        bool requireAuth = false)
    {
        var builder = endpoints.MapPost(pattern, handler);

        if (tags != null && tags.Length > 0)
            builder.WithTags(tags);

        if (!string.IsNullOrEmpty(summary))
            builder.WithSummary(summary);

        if (requireAuth)
            builder.RequireAuthorization();

        return builder.WithOpenApi();
    }

    /// <summary>
    /// Maps a PUT endpoint with consistent configuration
    /// </summary>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="handler">The request handler</param>
    /// <param name="tags">OpenAPI tags</param>
    /// <param name="summary">OpenAPI summary</param>
    /// <param name="requireAuth">Whether authentication is required</param>
    /// <returns>Route handler builder</returns>
    public static RouteHandlerBuilder MapApiPut(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate handler,
        string[]? tags = null,
        string? summary = null,
        bool requireAuth = false)
    {
        var builder = endpoints.MapPut(pattern, handler);

        if (tags != null && tags.Length > 0)
            builder.WithTags(tags);

        if (!string.IsNullOrEmpty(summary))
            builder.WithSummary(summary);

        if (requireAuth)
            builder.RequireAuthorization();

        return builder.WithOpenApi();
    }

    /// <summary>
    /// Maps a DELETE endpoint with consistent configuration
    /// </summary>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="handler">The request handler</param>
    /// <param name="tags">OpenAPI tags</param>
    /// <param name="summary">OpenAPI summary</param>
    /// <param name="requireAuth">Whether authentication is required</param>
    /// <returns>Route handler builder</returns>
    public static RouteHandlerBuilder MapApiDelete(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate handler,
        string[]? tags = null,
        string? summary = null,
        bool requireAuth = false)
    {
        var builder = endpoints.MapDelete(pattern, handler);

        if (tags != null && tags.Length > 0)
            builder.WithTags(tags);

        if (!string.IsNullOrEmpty(summary))
            builder.WithSummary(summary);

        if (requireAuth)
            builder.RequireAuthorization();

        return builder.WithOpenApi();
    }

    /// <summary>
    /// Maps a PATCH endpoint with consistent configuration
    /// </summary>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="handler">The request handler</param>
    /// <param name="tags">OpenAPI tags</param>
    /// <param name="summary">OpenAPI summary</param>
    /// <param name="requireAuth">Whether authentication is required</param>
    /// <returns>Route handler builder</returns>
    public static RouteHandlerBuilder MapApiPatch(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate handler,
        string[]? tags = null,
        string? summary = null,
        bool requireAuth = false)
    {
        var builder = endpoints.MapPatch(pattern, handler);

        if (tags != null && tags.Length > 0)
            builder.WithTags(tags);

        if (!string.IsNullOrEmpty(summary))
            builder.WithSummary(summary);

        if (requireAuth)
            builder.RequireAuthorization();

        return builder.WithOpenApi();
    }

    /// <summary>
    /// Maps a group of endpoints with common configuration
    /// </summary>
    /// <param name="endpoints">The endpoint route builder</param>
    /// <param name="prefix">The route prefix</param>
    /// <param name="configure">Action to configure the group</param>
    /// <param name="tags">Common tags for all endpoints in the group</param>
    /// <param name="requireAuth">Whether all endpoints in the group require authentication</param>
    /// <returns>Route group builder</returns>
    public static RouteGroupBuilder MapApiGroup(
        this IEndpointRouteBuilder endpoints,
        string prefix,
        Action<RouteGroupBuilder> configure,
        string[]? tags = null,
        bool requireAuth = false)
    {
        var group = endpoints.MapGroup(prefix);

        if (tags != null && tags.Length > 0)
            group.WithTags(tags);

        if (requireAuth)
            group.RequireAuthorization();

        group.WithOpenApi();
        ArgumentNullException.ThrowIfNull(configure);
        configure(group);

        return group;
    }

    /// <summary>
    /// Adds consistent error responses to an endpoint
    /// </summary>
    /// <param name="builder">The route handler builder</param>
    /// <param name="includeValidationErrors">Whether to include validation error responses</param>
    /// <returns>Route handler builder</returns>
    public static RouteHandlerBuilder WithStandardResponses(
        this RouteHandlerBuilder builder,
        bool includeValidationErrors = true)
    {
        builder.Produces<ProblemDetails>(500);
        
        if (includeValidationErrors)
        {
            builder.ProducesValidationProblem();
        }

        return builder;
    }

    /// <summary>
    /// Adds authentication responses to an endpoint
    /// </summary>
    /// <param name="builder">The route handler builder</param>
    /// <returns>Route handler builder</returns>
    public static RouteHandlerBuilder WithAuthResponses(this RouteHandlerBuilder builder)
    {
        return builder
            .Produces<ProblemDetails>(401)
            .Produces<ProblemDetails>(403);
    }
}