using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Asp.Versioning;
using Asp.Versioning.Builder;

namespace BuildingBlocks.API.Versioning.Extensions;

public static class VersionedEndpointExtensions
{
    public static RouteHandlerBuilder MapGetVersioned<T>(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Func<T, Task<IResult>> handler,
        ApiVersion version)
    {
        return endpoints.MapGet(pattern, handler)
            .HasApiVersion(version)
            .WithOpenApi();
    }

    public static RouteHandlerBuilder MapPostVersioned<T>(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Func<T, Task<IResult>> handler,
        ApiVersion version)
    {
        return endpoints.MapPost(pattern, handler)
            .HasApiVersion(version)
            .WithOpenApi();
    }

    public static RouteHandlerBuilder MapPutVersioned<T>(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Func<T, Task<IResult>> handler,
        ApiVersion version)
    {
        return endpoints.MapPut(pattern, handler)
            .HasApiVersion(version)
            .WithOpenApi();
    }

    public static RouteHandlerBuilder MapDeleteVersioned(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Func<Task<IResult>> handler,
        ApiVersion version)
    {
        return endpoints.MapDelete(pattern, handler)
            .HasApiVersion(version)
            .WithOpenApi();
    }

    public static RouteHandlerBuilder WithDeprecatedVersion(this RouteHandlerBuilder builder, ApiVersion version)
    {
        return builder.HasDeprecatedApiVersion(version);
    }

    public static RouteGroupBuilder WithDeprecatedVersion(this RouteGroupBuilder builder, ApiVersion version)
    {
        return builder.HasDeprecatedApiVersion(version);
    }

    public static RouteHandlerBuilder MapVersionedEndpoint(
        this IEndpointRouteBuilder endpoints,
        string method,
        string pattern,
        Delegate handler,
        ApiVersion version)
    {
        var routeBuilder = method.ToUpper() switch
        {
            "GET" => endpoints.MapGet(pattern, handler),
            "POST" => endpoints.MapPost(pattern, handler),
            "PUT" => endpoints.MapPut(pattern, handler),
            "DELETE" => endpoints.MapDelete(pattern, handler),
            "PATCH" => endpoints.MapPatch(pattern, handler),
            _ => throw new ArgumentException($"Unsupported HTTP method: {method}", nameof(method))
        };

        return routeBuilder
            .HasApiVersion(version)
            .WithOpenApi();
    }

    public static IResult ApiVersionNotSupported(string? message = null)
    {
        return Results.Problem(
            detail: message ?? "The requested API version is not supported.",
            statusCode: 400,
            title: "Unsupported API Version"
        );
    }

    public static IResult ApiVersionDeprecated(string? message = null, string? newVersion = null)
    {
        var detail = message ?? "This API version has been deprecated.";
        if (!string.IsNullOrEmpty(newVersion))
        {
            detail += $" Please use version {newVersion} instead.";
        }

        return Results.Problem(
            detail: detail,
            statusCode: 200, // Still works but deprecated
            title: "Deprecated API Version"
        );
    }
}