using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Data.Context;

public abstract class DbContextBase : DbContext, IDbContext
// ...existing code...
{
    public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
    {
        return base.Set<TEntity>();
    }
    protected DbContextBase(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);
        ApplyConfigurations(modelBuilder);
        ApplyConventions(modelBuilder);
    }

    protected virtual void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    protected virtual void ApplyConventions(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        // Apply naming conventions - simplified implementation
        // Can be extended based on specific requirements
    }
}