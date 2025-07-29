using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CustomerService.Domain.Entities;
using CustomerService.Domain.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace CustomerService.Infrastructure.Configurations;

public class CustomerContactNumberConfiguration : IEntityTypeConfiguration<CustomerContactNumber>
{
    public void Configure(EntityTypeBuilder<CustomerContactNumber> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table configuration
        builder.ToTable("CustomerContactNumbers", "customer");

        // Primary key
        builder.HasKey(cn => cn.Id);
        builder.Property(cn => cn.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(cn => cn.CustomerId)
            .IsRequired();

        builder.Property(cn => cn.ContactNumberTypeId)
            .IsRequired();

        builder.Property(cn => cn.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(cn => cn.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(cn => cn.Verified)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(cn => cn.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(cn => cn.UpdatedAt)
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.Property(cn => cn.UpdatedBy)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(cn => cn.CustomerId)
            .HasDatabaseName("IX_CustomerContactNumbers_CustomerId");

        builder.HasIndex(cn => cn.PhoneNumber)
            .HasDatabaseName("IX_CustomerContactNumbers_PhoneNumber");
    }
} 