namespace BuildingBlocks.API.OpenApi.Configuration;

public class ScalarOptions
{
    public string Title { get; set; } = "API Documentation";
    public string Description { get; set; } = "";
    public string Version { get; set; } = "v1";
    public string Theme { get; set; } = "purple";
    public bool ShowSidebar { get; set; } = true;
    public string SearchHotKey { get; set; } = "k";
    public string? CustomCss { get; set; }
    public string EndpointPathPrefix { get; set; } = "/scalar";
    public bool AlwaysShow { get; set; } = false;
    public string? OpenApiUrl { get; set; }
    public bool DarkMode { get; set; } = false;
    public bool HideModels { get; set; } = false;
    public string? LogoUrl { get; set; }
    public string? FaviconUrl { get; set; }
}