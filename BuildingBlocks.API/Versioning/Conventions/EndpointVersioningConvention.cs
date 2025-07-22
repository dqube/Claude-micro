using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Asp.Versioning;

namespace BuildingBlocks.API.Versioning.Conventions;

public static class EndpointVersioningConvention
{
    public static RouteGroupBuilder ApplyVersioningConventions(this RouteGroupBuilder group, string prefix = "api")
    {
        return group
            .WithTags("API")
            .WithOpenApi()
            .RequireAuthorization() // Default to requiring auth
            .AddEndpointFilter<VersionValidationFilter>();
    }

    public static RouteHandlerBuilder WithVersionSummary(this RouteHandlerBuilder builder, string summary, ApiVersion version)
    {
        return builder.WithSummary($"{summary} (v{version})");
    }

    public static RouteGroupBuilder WithVersionDescription(this RouteGroupBuilder builder, string description, ApiVersion version)
    {
        return builder.WithDescription($"{description} - API Version {version}");
    }

    public static RouteHandlerBuilder SupportsMultipleVersions(this RouteHandlerBuilder builder, params ApiVersion[] versions)
    {
        foreach (var version in versions)
        {
            builder.HasApiVersion(version);
        }
        return builder;
    }

    public static RouteHandlerBuilder DeprecatedInVersion(this RouteHandlerBuilder builder, ApiVersion deprecatedVersion, string? reason = null)
    {
        builder.HasDeprecatedApiVersion(deprecatedVersion);
        
        if (!string.IsNullOrEmpty(reason))
        {
            builder.WithDescription($"Deprecated in version {deprecatedVersion}: {reason}");
        }
        
        return builder;
    }
}

public class VersionValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var apiVersionFeature = httpContext.Features.Get<IApiVersioningFeature>();
        
        if (apiVersionFeature?.RequestedApiVersion == null)
        {
            return Results.Problem(
                detail: "API version is required but was not specified.",
                statusCode: 400,
                title: "Missing API Version"
            );
        }

        var requestedVersion = apiVersionFeature.RequestedApiVersion;
        var endpoint = httpContext.GetEndpoint();
        var supportedVersions = endpoint?.Metadata.GetMetadata<ApiVersionMetadata>();
        
        if (supportedVersions != null && !supportedVersions.IsApiVersionNeutral)
        {
            var isSupported = supportedVersions.SupportedApiVersions.Contains(requestedVersion) ||
                             supportedVersions.DeprecatedApiVersions.Contains(requestedVersion);
            
            if (!isSupported)
            {
                return Results.Problem(
                    detail: $"API version '{requestedVersion}' is not supported for this endpoint. Supported versions: {string.Join(", ", supportedVersions.SupportedApiVersions)}",
                    statusCode: 400,
                    title: "Unsupported API Version"
                );
            }
            
            // Add deprecation warning header if using deprecated version
            if (supportedVersions.DeprecatedApiVersions.Contains(requestedVersion))
            {
                httpContext.Response.Headers.Add("X-API-Deprecated", "true");
                httpContext.Response.Headers.Add("X-API-Deprecation-Info", 
                    $"Version {requestedVersion} is deprecated. Please upgrade to a supported version.");
            }
        }

        return await next(context);
    }
}