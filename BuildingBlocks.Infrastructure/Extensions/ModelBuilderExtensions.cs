using BuildingBlocks.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Configures Inbox and Outbox message entities with optional schema name
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    /// <param name="schemaName">Optional schema name. If null, uses default schema (dbo)</param>
    public static void ConfigureInboxOutbox(this ModelBuilder modelBuilder, string? schemaName = null)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration(schemaName));
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration(schemaName));
    }
}