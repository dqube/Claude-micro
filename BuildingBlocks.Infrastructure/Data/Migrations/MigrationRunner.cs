using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Infrastructure.Data.Context;

namespace BuildingBlocks.Infrastructure.Data.Migrations;

public partial class MigrationRunner : IMigrationRunner
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MigrationRunner> _logger;

    [LoggerMessage(LogLevel.Error, "Unable to resolve DbContext from IDbContext")]
    private static partial void LogDbContextResolutionError(ILogger logger);

    [LoggerMessage(LogLevel.Information, "Starting database migration")]
    private static partial void LogMigrationStarted(ILogger logger);

    [LoggerMessage(LogLevel.Information, "Database migration completed successfully")]
    private static partial void LogMigrationCompleted(ILogger logger);

    [LoggerMessage(LogLevel.Error, "Database migration failed")]
    private static partial void LogMigrationFailed(ILogger logger, Exception exception);

    [LoggerMessage(LogLevel.Information, "Starting database migration for {ContextType}")]
    private static partial void LogMigrationStartedForContext(ILogger logger, string contextType);

    [LoggerMessage(LogLevel.Information, "Database migration completed successfully for {ContextType}")]
    private static partial void LogMigrationCompletedForContext(ILogger logger, string contextType);

    [LoggerMessage(LogLevel.Error, "Database migration failed for {ContextType}")]
    private static partial void LogMigrationFailedForContext(ILogger logger, Exception exception, string contextType);

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
            LogDbContextResolutionError(_logger);
            return;
        }

        try
        {
            LogMigrationStarted(_logger);
            await context.Database.MigrateAsync(cancellationToken);
            LogMigrationCompleted(_logger);
        }
        catch (Exception ex)
        {
            LogMigrationFailed(_logger, ex);
            throw;
        }
    }

    public async Task MigrateAsync<TContext>(CancellationToken cancellationToken = default) where TContext : DbContext
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        
        try
        {
            LogMigrationStartedForContext(_logger, typeof(TContext).Name);
            await context.Database.MigrateAsync(cancellationToken);
            LogMigrationCompletedForContext(_logger, typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            LogMigrationFailedForContext(_logger, ex, typeof(TContext).Name);
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