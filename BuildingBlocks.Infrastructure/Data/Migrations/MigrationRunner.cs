using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Infrastructure.Data.Context;

namespace BuildingBlocks.Infrastructure.Data.Migrations;

public class MigrationRunner : IMigrationRunner
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MigrationRunner> _logger;

    public MigrationRunner(IServiceProvider serviceProvider, ILogger<MigrationRunner> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IDbContext>() as DbContext;
        
        if (context == null)
        {
            _logger.LogError("Unable to resolve DbContext from IDbContext");
            return;
        }

        try
        {
            _logger.LogInformation("Starting database migration");
            await context.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database migration failed");
            throw;
        }
    }

    public async Task MigrateAsync<TContext>(CancellationToken cancellationToken = default) where TContext : DbContext
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        
        try
        {
            _logger.LogInformation("Starting database migration for {ContextType}", typeof(TContext).Name);
            await context.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Database migration completed successfully for {ContextType}", typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database migration failed for {ContextType}", typeof(TContext).Name);
            throw;
        }
    }

    public async Task<bool> HasPendingMigrationsAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IDbContext>() as DbContext;
        
        if (context == null)
            return false;
        
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
        return pendingMigrations.Any();
    }

    public async Task<bool> HasPendingMigrationsAsync<TContext>(CancellationToken cancellationToken = default) where TContext : DbContext
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
        return pendingMigrations.Any();
    }

    public async Task<IEnumerable<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IDbContext>() as DbContext;
        
        if (context == null)
            return Enumerable.Empty<string>();
        
        return await context.Database.GetPendingMigrationsAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetPendingMigrationsAsync<TContext>(CancellationToken cancellationToken = default) where TContext : DbContext
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        
        return await context.Database.GetPendingMigrationsAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetAppliedMigrationsAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IDbContext>() as DbContext;
        
        if (context == null)
            return Enumerable.Empty<string>();
            
        return await context.Database.GetAppliedMigrationsAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetAppliedMigrationsAsync<TContext>(CancellationToken cancellationToken = default) where TContext : DbContext
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        
        return await context.Database.GetAppliedMigrationsAsync(cancellationToken);
    }
}