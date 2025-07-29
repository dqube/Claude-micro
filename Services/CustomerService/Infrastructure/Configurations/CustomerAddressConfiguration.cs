using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CustomerService.Domain.Entities;
using CustomerService.Domain.ValueObjects;

namespace CustomerService.Infrastructure.Configurations;

public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table configuration
        builder.ToTable("CustomerAddresses", "customer");

        // Primary key
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(a => a.CustomerId)
            .IsRequired();

        builder.Property(a => a.AddressTypeId)
            .IsRequired();

        builder.Property(a => a.Line1)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Line2)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(a => a.City)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.State)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(a => a.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.CountryCode)
            .IsRequired()
            .HasMaxLength(2)
            .IsFixedLength();

        builder.Property(a => a.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(a => a.UpdatedAt)
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.Property(a => a.UpdatedBy)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(a => a.CustomerId)
            .HasDatabaseName("IX_CustomerAddresses_CustomerId");

        builder.HasIndex(a => a.CountryCode)
            .HasDatabaseName("IX_CustomerAddresses_CountryCode");
    }
} 