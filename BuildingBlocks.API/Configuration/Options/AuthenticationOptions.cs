using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.API.Configuration.Options;

public class AuthenticationOptions
{
    public const string ConfigurationSection = "Authentication";

    public JwtOptions Jwt { get; set; } = new();
    public ApiKeyOptions ApiKey { get; set; } = new();
}

public class JwtOptions
{
    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    public string? SecretKey { get; set; }

    public string? Authority { get; set; }

    public bool RequireHttpsMetadata { get; set; } = true;

    public bool SaveToken { get; set; } = false;

    public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromHours(1);

    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(5);
}

public class ApiKeyOptions
{
    public bool Enabled { get; set; } = false;

    public string HeaderName { get; set; } = "X-API-Key";

    public string QueryParameterName { get; set; } = "apiKey";

    public string[] ValidApiKeys { get; set; } = Array.Empty<string>();
}