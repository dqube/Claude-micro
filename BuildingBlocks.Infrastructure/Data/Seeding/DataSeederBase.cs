using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Data.Seeding;

public abstract class DataSeederBase : IDataSeeder
{
    protected readonly ILogger Logger;

    protected DataSeederBase(ILogger logger)
    {
        Logger = logger;
    }

    public abstract Task SeedAsync(CancellationToken cancellationToken = default);
    public abstract int Order { get; }

    protected async Task SeedIfNotExistsAsync<T>(
        Func<Task<bool>> existsCheck,
        Func<Task<T>> seedData,
        string entityName)
    {
        if (!await existsCheck())
        {
            Logger.LogInformation("Seeding {EntityName} data", entityName);
            await seedData();
            Logger.LogInformation("Successfully seeded {EntityName} data", entityName);
        }
        else
        {
            Logger.LogInformation("{EntityName} data already exists, skipping seed", entityName);
        }
    }
}