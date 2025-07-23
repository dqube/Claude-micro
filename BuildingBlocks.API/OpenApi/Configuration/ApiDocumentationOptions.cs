namespace BuildingBlocks.API.OpenApi.Configuration;

public class ApiDocumentationOptions
{
    public string Title { get; set; } = "API Documentation";
    public string Version { get; set; } = "v1";
    public string Description { get; set; } = "API Documentation";
    public bool EnableScalar { get; set; } = true;
    public bool IncludeXmlComments { get; set; } = true;
    public ContactInfo Contact { get; set; } = new();
    public LicenseInfo License { get; set; } = new();
    public SecurityDefinition Security { get; set; } = new();
}

public class ContactInfo
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Uri Url { get; set; } = new Uri("about:blank");
}

public class LicenseInfo
{
    public string Name { get; set; } = string.Empty;
    public Uri Url { get; set; } = new Uri("about:blank");
}

public class SecurityDefinition
{
    public bool EnableJwt { get; set; } = true;
    public bool EnableApiKey { get; set; } = false;
    public string ApiKeyHeaderName { get; set; } = "X-API-Key";
}