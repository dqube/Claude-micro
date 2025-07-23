using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Data.Seeding;

public abstract class DataSeederBase : IDataSeeder
{
    protected readonly ILogger Logger;

    protected DataSeederBase(ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        Logger = logger;
    }

    public abstract Task SeedAsync(CancellationToken cancellationToken = default);
    public abstract int Order { get; }

    protected async Task SeedIfNotExistsAsync<T>(
        Func<Task<bool>> existsCheck,
        Func<Task<T>> seedData,
        string entityName)
    {
        ArgumentNullException.ThrowIfNull(existsCheck);
        ArgumentNullException.ThrowIfNull(seedData);

        if (!await existsCheck())
        {
            LogSeeding(Logger, entityName, null);
            await seedData();
            LogSeeded(Logger, entityName, null);
        }
        else
        {
            LogAlreadyExists(Logger, entityName, null);
        }
    }

    private static readonly Action<ILogger, string, Exception?> LogSeeding =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(1001, "Seeding"), "Seeding {EntityName} data");

    private static readonly Action<ILogger, string, Exception?> LogSeeded =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(1002, "Seeded"), "Successfully seeded {EntityName} data");

    private static readonly Action<ILogger, string, Exception?> LogAlreadyExists =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(1003, "AlreadyExists"), "{EntityName} data already exists, skipping seed");
}