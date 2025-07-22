using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.API.Configuration.Options;

public class CorsOptions
{
    public const string ConfigurationSection = "Cors";

    [Required]
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

    public string[] AllowedMethods { get; set; } = ["GET", "POST", "PUT", "DELETE", "OPTIONS"];

    public string[] AllowedHeaders { get; set; } = ["*"];

    public string[] ExposedHeaders { get; set; } = Array.Empty<string>();

    public bool AllowCredentials { get; set; } = false;

    public int PreflightMaxAge { get; set; } = 86400; // 24 hours

    public string PolicyName { get; set; } = "DefaultCorsPolicy";
}