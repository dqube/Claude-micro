namespace BuildingBlocks.API.Configuration.Options;

public class ApiOptions
{
    public const string SectionName = "Api";

    public string Title { get; set; } = "API";
    public string Version { get; set; } = "v1";
    public string Description { get; set; } = "API Description";
    public Uri? ContactUrl { get; set; }
    public Uri? LicenseUrl { get; set; }
    public Uri? TermsOfServiceUrl { get; set; }
    public string LicenseName { get; set; } = string.Empty;
    public bool IncludeXmlComments { get; set; } = true;
    public bool EnableScalar { get; set; } = true;
}

public class CorsOptions
{
    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; set; } = [];
    public string[] AllowedMethods { get; set; } = ["GET", "POST", "PUT", "DELETE", "PATCH"];
    public string[] AllowedHeaders { get; set; } = ["*"];
    public string[] ExposedHeaders { get; set; } = [];
    public bool AllowCredentials { get; set; } = false;
    public int PreflightMaxAge { get; set; } = 86400; // 24 hours
}

public class AuthenticationOptions
{
    public const string SectionName = "Authentication";

    public JwtOptions Jwt { get; set; } = new();
    public ApiKeyOptions ApiKey { get; set; } = new();
}

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience { get; set; } = true;
    public bool ValidateLifetime { get; set; } = true;
    public bool ValidateIssuerSigningKey { get; set; } = true;
    public int ClockSkewMinutes { get; set; } = 5;
}

public class ApiKeyOptions
{
    public string HeaderName { get; set; } = "X-API-Key";
    public string[] ValidApiKeys { get; set; } = [];
}

public class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public int PermitLimit { get; set; } = 100;
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);
    public bool Enabled { get; set; } = true;
    public string[] ExcludedPaths { get; set; } = ["/health"];
}

public class InboxOutboxOptions
{
    public const string SectionName = "InboxOutbox";

    public bool Enabled { get; set; } = true;
    public bool IncludeInboxService { get; set; } = true;
    public bool IncludeOutboxService { get; set; } = true;
}