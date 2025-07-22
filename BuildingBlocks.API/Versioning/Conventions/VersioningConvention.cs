using Microsoft.AspNetCore.Builder;
using Asp.Versioning;
using Asp.Versioning.Conventions;

namespace BuildingBlocks.API.Versioning.Conventions;

public static class VersioningConvention
{
    public static ApiVersionConventionBuilder ApplyVersioningConventions(this ApiVersionConventionBuilder builder)
    {
        // Default conventions for common endpoints
        builder.Controller("Health").HasApiVersions(1.0, 2.0);
        builder.Controller("Auth").HasApiVersions(1.0, 2.0);
        
        return builder;
    }

    public static void ConfigureDefaultVersioning(ApiVersioningOptions options)
    {
        options.DefaultApiVersion = new ApiVersion(1.0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        
        // Support multiple ways to specify version
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),           // /v1/users
            new QueryStringApiVersionReader("version"), // ?version=1.0
            new HeaderApiVersionReader("X-Version"),    // X-Version: 1.0
            new MediaTypeApiVersionReader("version")    // Accept: application/json;version=1.0
        );
        
        // Use current implementation when version is ambiguous
        options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
    }

    public static void ConfigureConservativeVersioning(ApiVersioningOptions options)
    {
        options.DefaultApiVersion = new ApiVersion(1.0);
        options.AssumeDefaultVersionWhenUnspecified = false; // Force explicit versioning
        
        // Only support URL segment versioning
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
        
        // Use lowest version when ambiguous
        options.ApiVersionSelector = new LowestImplementedApiVersionSelector(options);
    }

    public static void ConfigureFlexibleVersioning(ApiVersioningOptions options)
    {
        options.DefaultApiVersion = new ApiVersion(1.0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        
        // Support all versioning methods
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new QueryStringApiVersionReader("version"),
            new QueryStringApiVersionReader("api-version"),
            new HeaderApiVersionReader("X-Version"),
            new HeaderApiVersionReader("API-Version"),
            new MediaTypeApiVersionReader("version")
        );
        
        options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
    }
}