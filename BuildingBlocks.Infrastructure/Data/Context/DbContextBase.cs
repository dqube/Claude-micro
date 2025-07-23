using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Data.Context;

public abstract class DbContextBase : DbContext, IDbContext
{
    protected DbContextBase(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
        // Apply naming conventions - simplified implementation
        // Can be extended based on specific requirements
    }
}