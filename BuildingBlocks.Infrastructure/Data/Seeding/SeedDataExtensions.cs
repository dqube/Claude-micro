using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Data.Seeding;

public static partial class SeedDataExtensions
{
    [LoggerMessage(LogLevel.Information, "No data seeders found")]
    private static partial void LogNoSeedersFound(ILogger logger);

    [LoggerMessage(LogLevel.Information, "Starting data seeding with {SeederCount} seeders")]
    private static partial void LogSeedingStarted(ILogger logger, int seederCount);

    [LoggerMessage(LogLevel.Information, "Running seeder: {SeederType} (Order: {Order})")]
    private static partial void LogSeederStarted(ILogger logger, string seederType, int order);

    [LoggerMessage(LogLevel.Information, "Completed seeder: {SeederType}")]
    private static partial void LogSeederCompleted(ILogger logger, string seederType);

    [LoggerMessage(LogLevel.Error, "Failed to run seeder: {SeederType}")]
    private static partial void LogSeederFailed(ILogger logger, Exception exception, string seederType);

    [LoggerMessage(LogLevel.Information, "Data seeding completed successfully")]
    private static partial void LogSeedingCompleted(ILogger logger);

    public static async Task<IHost> SeedDataAsync(this IHost host)
    {
        ArgumentNullException.ThrowIfNull(host);
        using var scope = host.Services.CreateScope();
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>()
            .OrderBy(s => s.Order)
            .ToList();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<IDataSeeder>>();

        if (seeders.Count == 0)
        {
            LogNoSeedersFound(logger);
            return host;
        }

        LogSeedingStarted(logger, seeders.Count);

        foreach (var seeder in seeders)
        {
            try
            {
                LogSeederStarted(logger, seeder.GetType().Name, seeder.Order);
                
                await seeder.SeedAsync();
                
                LogSeederCompleted(logger, seeder.GetType().Name);
            }
            catch (Exception ex)
            {
                LogSeederFailed(logger, ex, seeder.GetType().Name);
                throw;
            }
        }

        LogSeedingCompleted(logger);
        return host;
    }

    public static IServiceCollection AddDataSeeder<T>(this IServiceCollection services)
        where T : class, IDataSeeder
    {
        return services.AddScoped<IDataSeeder, T>();
    }

    public static IServiceCollection AddDataSeeders(this IServiceCollection services, params Type[] seederTypes)
    {
        ArgumentNullException.ThrowIfNull(seederTypes);
        foreach (var seederType in seederTypes)
        {
            if (!typeof(IDataSeeder).IsAssignableFrom(seederType))
            {
                throw new ArgumentException($"Type {seederType.Name} does not implement IDataSeeder");
            }

            services.AddScoped(typeof(IDataSeeder), seederType);
        }

        return services;
    }
}