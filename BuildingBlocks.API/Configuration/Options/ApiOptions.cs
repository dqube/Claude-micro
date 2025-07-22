using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.API.Configuration.Options;

public class ApiOptions
{
    public const string ConfigurationSection = "Api";

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Version { get; set; } = "v1";

    public string Description { get; set; } = string.Empty;

    public string? TermsOfService { get; set; }

    public ContactInfo? Contact { get; set; }

    public LicenseInfo? License { get; set; }

    public bool EnableSwagger { get; set; } = true;

    public bool EnableScalar { get; set; } = true;
}

public class ContactInfo
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Url { get; set; }
}

public class LicenseInfo
{
    public string? Name { get; set; }
    public string? Url { get; set; }
}