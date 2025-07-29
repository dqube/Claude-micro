using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CustomerService.Domain.Entities;
using CustomerService.Domain.ValueObjects;
using BuildingBlocks.Domain.Common;

namespace CustomerService.Infrastructure.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Table configuration
        builder.ToTable("Customers", "customer");

        // Primary key
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        // Properties
        builder.Property(c => c.UserId)
            .IsRequired(false);

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Email)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(c => c.MembershipNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.JoinDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(c => c.ExpiryDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(c => c.CountryCode)
            .IsRequired()
            .HasMaxLength(2)
            .IsFixedLength();

        builder.Property(c => c.LoyaltyPoints)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.PreferredContactMethod)
            .IsRequired(false);

        builder.Property(c => c.PreferredAddressType)
            .IsRequired(false);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(c => c.CreatedBy)
            .IsRequired(false);

        builder.Property(c => c.UpdatedAt)
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.Property(c => c.UpdatedBy)
            .IsRequired(false);

        // Relationships
        builder.HasMany(c => c.ContactNumbers)
            .WithOne(cn => cn.Customer)
            .HasForeignKey(cn => cn.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Addresses)
            .WithOne(a => a.Customer)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(c => c.Email)
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL")
            .HasDatabaseName("IX_Customers_Email");

        builder.HasIndex(c => c.MembershipNumber)
            .IsUnique()
            .HasDatabaseName("IX_Customers_MembershipNumber");

        builder.HasIndex(c => c.CountryCode)
            .HasDatabaseName("IX_Customers_CountryCode");

        builder.HasIndex(c => c.JoinDate)
            .HasDatabaseName("IX_Customers_JoinDate");

        builder.HasIndex(c => c.ExpiryDate)
            .HasDatabaseName("IX_Customers_ExpiryDate");
    }
} 