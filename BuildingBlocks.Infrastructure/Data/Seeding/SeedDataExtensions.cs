using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Data.Seeding;

public static class SeedDataExtensions
{
    public static async Task<IHost> SeedDataAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>()
            .OrderBy(s => s.Order)
            .ToList();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<IDataSeeder>>();

        if (!seeders.Any())
        {
            logger.LogInformation("No data seeders found");
            return host;
        }

        logger.LogInformation("Starting data seeding with {SeederCount} seeders", seeders.Count);

        foreach (var seeder in seeders)
        {
            try
            {
                logger.LogInformation("Running seeder: {SeederType} (Order: {Order})", 
                    seeder.GetType().Name, seeder.Order);
                
                await seeder.SeedAsync();
                
                logger.LogInformation("Completed seeder: {SeederType}", seeder.GetType().Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run seeder: {SeederType}", seeder.GetType().Name);
                throw;
            }
        }

        logger.LogInformation("Data seeding completed successfully");
        return host;
    }

    public static IServiceCollection AddDataSeeder<T>(this IServiceCollection services)
        where T : class, IDataSeeder
    {
        return services.AddScoped<IDataSeeder, T>();
    }

    public static IServiceCollection AddDataSeeders(this IServiceCollection services, params Type[] seederTypes)
    {
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