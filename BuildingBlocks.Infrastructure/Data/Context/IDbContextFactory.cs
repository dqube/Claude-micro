namespace BuildingBlocks.Infrastructure.Data.Context;

public interface IDbContextFactory<out TDbContext> where TDbContext : class, IDbContext
{
    TDbContext CreateDbContext();
}

public interface IDbContextFactory
{
    TDbContext CreateDbContext<TDbContext>() where TDbContext : class, IDbContext;
    IDbContext CreateDbContext(Type dbContextType);
}