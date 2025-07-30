using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Infrastructure.Data.Context;
using SharedService.Domain.Entities;

namespace SharedService.Infrastructure.Persistence;

public class SharedDbContext : DbContextBase
{
    public SharedDbContext(DbContextOptions<SharedDbContext> options) : base(options)
    {
    }

    public DbSet<Currency> Currencies { get; set; } = null!;
    public DbSet<Country> Countries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SharedDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
} 