using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Data.Migrations;

public interface IMigrationRunner
{
    Task MigrateAsync(CancellationToken cancellationToken = default);
    Task MigrateAsync<TContext>(CancellationToken cancellationToken = default) where TContext : DbContext;
    Task<bool> HasPendingMigrationsAsync(CancellationToken cancellationToken = default);
    Task<bool> HasPendingMigrationsAsync<TContext>(CancellationToken cancellationToken = default) where TContext : DbContext;
    Task<IEnumerable<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetPendingMigrationsAsync<TContext>(CancellationToken cancellationToken = default) where TContext : DbContext;
    Task<IEnumerable<string>> GetAppliedMigrationsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetAppliedMigrationsAsync<TContext>(CancellationToken cancellationToken = default) where TContext : DbContext;
}