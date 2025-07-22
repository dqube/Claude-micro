namespace BuildingBlocks.API.OpenApi.Configuration;

public class ApiDocumentationOptions
{
    public const string ConfigurationSection = "ApiDocumentation";

    public bool EnableSwagger { get; set; } = true;
    public bool EnableScalar { get; set; } = true;
    public bool EnableReDoc { get; set; } = false;
    public string RoutePrefix { get; set; } = "docs";
    public string SwaggerRoutePrefix { get; set; } = "swagger";
    public string ScalarRoutePrefix { get; set; } = "scalar";
    public string ReDocRoutePrefix { get; set; } = "redoc";
    public bool IncludeXmlComments { get; set; } = true;
    public string[] XmlCommentsFiles { get; set; } = Array.Empty<string>();
    public SecuritySchemeOptions Security { get; set; } = new();
    public ServerOptions[] Servers { get; set; } = Array.Empty<ServerOptions>();
    public ExternalDocsOptions? ExternalDocs { get; set; }
    public TagOptions[] Tags { get; set; } = Array.Empty<TagOptions>();
    public bool SortTagsAlphabetically { get; set; } = true;
    public bool DisplayRequestDuration { get; set; } = true;
    public bool EnableTryItOut { get; set; } = true;
    public string DefaultExpansion { get; set; } = "list"; // none, list, full
    public int DefaultModelExpandDepth { get; set; } = 1;
    public bool ShowExtensions { get; set; } = false;
    public bool ShowCommonExtensions { get; set; } = true;
    public ThemeOptions Theme { get; set; } = new();
}

public class SecuritySchemeOptions
{
    public bool EnableJwtBearer { get; set; } = true;
    public bool EnableApiKey { get; set; } = true;
    public bool EnableOAuth2 { get; set; } = false;
    public string JwtBearerDescription { get; set; } = "Enter your JWT token in the format: Bearer {your token}";
    public string ApiKeyHeaderName { get; set; } = "X-API-Key";
    public string ApiKeyDescription { get; set; } = "API Key needed to access the endpoints";
    public OAuth2Options OAuth2 { get; set; } = new();
}

public class OAuth2Options
{
    public string AuthorizationUrl { get; set; } = string.Empty;
    public string TokenUrl { get; set; } = string.Empty;
    public string RefreshUrl { get; set; } = string.Empty;
    public Dictionary<string, string> Scopes { get; set; } = new();
}

public class ServerOptions
{
    public string Url { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, ServerVariableOptions> Variables { get; set; } = new();
}

public class ServerVariableOptions
{
    public string[] Enum { get; set; } = Array.Empty<string>();
    public string Default { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ExternalDocsOptions
{
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class TagOptions
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ExternalDocsOptions? ExternalDocs { get; set; }
}

public class ThemeOptions
{
    public string PrimaryColor { get; set; } = "#3b82f6";
    public string BackgroundColor { get; set; } = "#ffffff";
    public string TextColor { get; set; } = "#374151";
    public string FontFamily { get; set; } = "Inter, system-ui, sans-serif";
    public bool DarkMode { get; set; } = false;
    public string LogoUrl { get; set; } = string.Empty;
    public string FaviconUrl { get; set; } = string.Empty;
}