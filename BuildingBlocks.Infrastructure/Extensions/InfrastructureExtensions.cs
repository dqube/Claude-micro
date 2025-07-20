using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using BuildingBlocks.Infrastructure.Configuration;
using BuildingBlocks.Infrastructure.Logging;

namespace BuildingBlocks.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IConfigurationService, ConfigurationService>();
        services.AddScoped<ILoggerService, LoggerService>();
        
        return services;
    }

    public static IServiceCollection AddAllInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddMemoryCache();
        
        return services;
    }
}