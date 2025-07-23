using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Asp.Versioning;

namespace BuildingBlocks.API.Versioning.Conventions;

public static class VersioningConvention
{
    public static void ApplyVersioning(EndpointBuilder builder, ApiVersion version)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(version);
        
        builder.Metadata.Add(version);
        builder.DisplayName = $"{builder.DisplayName}_v{version.MajorVersion}";
    }

    public static RouteGroupBuilder WithApiVersion(this RouteGroupBuilder group, int majorVersion, int minorVersion = 0)
    {
        var version = new ApiVersion(majorVersion, minorVersion);
        return group.WithMetadata(version);
    }

    public static RouteHandlerBuilder WithApiVersion(this RouteHandlerBuilder builder, int majorVersion, int minorVersion = 0)
    {
        var version = new ApiVersion(majorVersion, minorVersion);
        return builder.WithMetadata(version);
    }
}

public class EndpointVersioningConvention
{
    public static void Configure(EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        // Simple versioning convention based on endpoint metadata
        // This would be used by the routing system to apply version metadata
        var version = new ApiVersion(1, 0);
        builder.Metadata.Add(version);
    }
}