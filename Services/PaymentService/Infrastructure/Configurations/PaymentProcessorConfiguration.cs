using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Infrastructure.Configurations;

public class PaymentProcessorConfiguration : IEntityTypeConfiguration<PaymentProcessor>
{
    public void Configure(EntityTypeBuilder<PaymentProcessor> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("PaymentProcessors", "payment");
        builder.HasKey(p => p.Id);

        // Name
        builder.Property(p => p.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(p => p.Name)
            .IsUnique()
            .HasDatabaseName("IX_PaymentProcessors_Name");

        // IsActive
        builder.Property(p => p.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        // CommissionRate
        builder.Property(p => p.CommissionRate)
            .HasColumnType("DECIMAL(5,2)")
            .HasDefaultValue(0)
            .IsRequired();

        // Audit fields
        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(p => p.CreatedBy);

        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.UpdatedBy);

        // Additional indexes
        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_PaymentProcessors_IsActive");

        builder.HasIndex(p => p.CreatedAt)
            .HasDatabaseName("IX_PaymentProcessors_CreatedAt");
    }
} 