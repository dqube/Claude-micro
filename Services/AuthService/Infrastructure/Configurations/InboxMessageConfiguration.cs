using BuildingBlocks.Application.Inbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("InboxMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.MessageId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.MessageType)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Payload)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.Source)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.ReceivedAt)
            .IsRequired();

        builder.Property(x => x.ProcessedAt)
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

        // Indexes for performance
        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_InboxMessages_Status");

        builder.HasIndex(x => x.ReceivedAt)
            .HasDatabaseName("IX_InboxMessages_ReceivedAt");

        builder.HasIndex(x => x.MessageId)
            .IsUnique()
            .HasDatabaseName("IX_InboxMessages_MessageId");

        builder.HasIndex(x => x.MessageType)
            .HasDatabaseName("IX_InboxMessages_MessageType");
    }
}