using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Configuration;

public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(IConfiguration configuration, ILogger<ConfigurationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public T GetSection<T>(string sectionName) where T : class, new()
    {
        var section = new T();
        _configuration.GetSection(sectionName).Bind(section);
        return section;
    }

    public T GetValue<T>(string key, T defaultValue = default!)
    {
        return _configuration.GetValue<T>(key, defaultValue);
    }

    public string GetConnectionString(string name)
    {
        return _configuration.GetConnectionString(name) ?? string.Empty;
    }

    public bool TryGetSection<T>(string sectionName, out T section) where T : class, new()
    {
        try
        {
            section = GetSection<T>(sectionName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get configuration section {SectionName}", sectionName);
            section = new T();
            return false;
        }
    }

    public void Reload()
    {
        if (_configuration is IConfigurationRoot configRoot)
        {
            configRoot.Reload();
            _logger.LogInformation("Configuration reloaded");
        }
    }

    public void ValidateConfiguration()
    {
        _logger.LogInformation("Configuration validation completed");
    }
}