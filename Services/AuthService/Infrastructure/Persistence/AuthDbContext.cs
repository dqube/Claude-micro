using BuildingBlocks.Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Configurations;

namespace AuthService.Infrastructure.Persistence;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RegistrationToken> RegistrationTokens => Set<RegistrationToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new RegistrationTokenConfiguration());

        // Configure all strongly typed IDs automatically
        modelBuilder.ConfigureStronglyTypedIds();

        // Ignore domain events for EF Core
        modelBuilder.Entity<User>()
            .Ignore(u => u.DomainEvents);
        
        modelBuilder.Entity<RegistrationToken>()
            .Ignore(rt => rt.DomainEvents);
            
        modelBuilder.Entity<UserRole>()
            .Ignore(ur => ur.DomainEvents);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);
        if (!optionsBuilder.IsConfigured)
        {
            // This will be overridden by the options passed in the constructor
            // but we include it here for completeness
        }

        // Enable strongly typed ID value converters
        optionsBuilder.UseStronglyTypedIdConverters();
        
        base.OnConfiguring(optionsBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Publish domain events before saving changes
        await PublishDomainEventsAsync(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        var userEntities = ChangeTracker
            .Entries<User>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();
            
        var tokenEntities = ChangeTracker
            .Entries<RegistrationToken>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();
            
        var userRoleEntities = ChangeTracker
            .Entries<UserRole>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var allDomainEvents = userEntities
            .SelectMany(x => x.DomainEvents)
            .Concat(tokenEntities.SelectMany(x => x.DomainEvents))
            .Concat(userRoleEntities.SelectMany(x => x.DomainEvents))
            .ToList();

        // Clear domain events from entities
        userEntities.ForEach(entity => entity.ClearDomainEvents());
        tokenEntities.ForEach(entity => entity.ClearDomainEvents());
        userRoleEntities.ForEach(entity => entity.ClearDomainEvents());

        foreach (var domainEvent in allDomainEvents)
        {
            // Here you would publish the domain event using your preferred method
            // For example, using MediatR or a service bus
            // await _mediator.Publish(domainEvent, cancellationToken);
            await Task.CompletedTask;
        }
    }
}