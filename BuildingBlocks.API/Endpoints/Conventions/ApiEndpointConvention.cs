using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace BuildingBlocks.API.Endpoints.Conventions;

public class ApiEndpointConvention : IEndpointConventionBuilder
{
    private readonly IEndpointConventionBuilder _builder;

    public ApiEndpointConvention(IEndpointConventionBuilder builder)
    {
        _builder = builder;
    }

    public void Add(Action<EndpointBuilder> convention)
    {
        _builder.Add(convention);
    }

    public void Finally(Action<EndpointBuilder> finallyConvention)
    {
        _builder.Finally(finallyConvention);
    }

    public static void ApplyConventions(EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        // Apply consistent conventions across all API endpoints
        
        // Simplified metadata without complex type mappings
        builder.DisplayName ??= "API Endpoint";
    }
}

public class VersioningEndpointConvention : IEndpointConventionBuilder
{
    private readonly IEndpointConventionBuilder _builder;

    public VersioningEndpointConvention(IEndpointConventionBuilder builder)
    {
        _builder = builder;
    }

    public void Add(Action<EndpointBuilder> convention)
    {
        _builder.Add(convention);
    }

    public void Finally(Action<EndpointBuilder> finallyConvention)
    {
        _builder.Finally(finallyConvention);
    }

    public static void ApplyVersioningConventions(EndpointBuilder builder, int majorVersion, int minorVersion = 0)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        var apiVersion = new ApiVersion(majorVersion, minorVersion);
        builder.Metadata.Add(new ApiVersionMetadata(apiVersion));
        
        // Simplified versioning without EndpointNameMetadata dependency
        builder.DisplayName = $"{builder.DisplayName}_v{majorVersion}_{minorVersion}";
    }
}

// Helper metadata classes
public class ApiVersionMetadata
{
    public ApiVersion Version { get; }

    public ApiVersionMetadata(ApiVersion version)
    {
        Version = version;
    }
}