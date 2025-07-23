using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Data.Context;

public class ApplicationDbContext : DbContextBase
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);
        // Configure entity relationships and constraints here
        ConfigureEntities(modelBuilder);
    }

    private static void ConfigureEntities(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        // Add specific entity configurations here
    }
}