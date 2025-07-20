namespace BuildingBlocks.Infrastructure.Data.Seeding;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
    int Order { get; }
}