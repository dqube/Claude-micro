using BuildingBlocks.Application.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Infrastructure.Data.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    private readonly string? _schemaName;

    public OutboxMessageConfiguration(string? schemaName = null)
    {
        _schemaName = schemaName;
    }

    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Set table name with optional schema
        if (!string.IsNullOrEmpty(_schemaName))
        {
            builder.ToTable("OutboxMessages", _schemaName);
        }
        else
        {
            builder.ToTable("OutboxMessages");
        }

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.MessageType)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Payload)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.Destination)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.CorrelationId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.PublishedAt)
            .IsRequired(false);

        builder.Property(x => x.LastAttemptAt)
            .IsRequired(false);

        builder.Property(x => x.RetryCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.ErrorMessage)
            .IsRequired(false)
            .HasMaxLength(2000);

        builder.Property(x => x.StackTrace)
            .IsRequired(false)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.ScheduledAt)
            .IsRequired(false);

        // Indexes for performance
        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_OutboxMessages_Status");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_OutboxMessages_CreatedAt");

        builder.HasIndex(x => new { x.Status, x.ScheduledAt })
            .HasDatabaseName("IX_OutboxMessages_Status_ScheduledAt");

        builder.HasIndex(x => x.CorrelationId)
            .HasDatabaseName("IX_OutboxMessages_CorrelationId");
    }
}