namespace BuildingBlocks.Infrastructure.Configuration;

public interface IConfigurationService
{
    T GetSection<T>(string sectionName) where T : class, new();
    T GetValue<T>(string key, T defaultValue = default!);
    string GetConnectionString(string name);
    bool TryGetSection<T>(string sectionName, out T section) where T : class, new();
    void Reload();
    void ValidateConfiguration();
}