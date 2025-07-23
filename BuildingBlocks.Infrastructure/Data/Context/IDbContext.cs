using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BuildingBlocks.Infrastructure.Data.Context;

public interface IDbContext : IDisposable
{
    /// <summary>
    /// Gets a DbSet for the given entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>A DbSet for the given entity type.</returns>
    DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class;

    /// <summary>
    /// Gets an EntityEntry for the given entity.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entity">The entity instance.</param>
    /// <returns>The entity entry.</returns>
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Gets the ChangeTracker for the context.
    /// </summary>
    ChangeTracker ChangeTracker { get; }

    /// <summary>
    /// Gets the DatabaseFacade for the context.
    /// </summary>
    DatabaseFacade Database { get; }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    int SaveChanges();

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database, optionally accepting all changes on success.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Whether to accept all changes on success.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes made in this context to the database, optionally accepting all changes on success.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Whether to accept all changes on success.</param>
    /// <returns>The number of state entries written to the database.</returns>
    int SaveChanges(bool acceptAllChangesOnSuccess);
}