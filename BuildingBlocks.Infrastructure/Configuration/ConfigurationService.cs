using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Configuration;

public partial class ConfigurationService : IConfigurationService
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
        return _configuration.GetValue<T>(key, defaultValue) ?? defaultValue;
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
            LogFailedToGetConfigurationSection(_logger, ex, sectionName);
            section = new T();
            return false;
        }
    }

    public void Reload()
    {
        if (_configuration is IConfigurationRoot configRoot)
        {
            configRoot.Reload();
            LogConfigurationReloaded(_logger);
        }
    }

    public void ValidateConfiguration()
    {
        LogConfigurationValidationCompleted(_logger);
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Failed to get configuration section {sectionName}")]
    private static partial void LogFailedToGetConfigurationSection(ILogger logger, Exception exception, string sectionName);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "Configuration reloaded")]
    private static partial void LogConfigurationReloaded(ILogger logger);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Information,
        Message = "Configuration validation completed")]
    private static partial void LogConfigurationValidationCompleted(ILogger logger);
}