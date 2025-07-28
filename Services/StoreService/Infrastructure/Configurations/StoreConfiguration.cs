using BuildingBlocks.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreService.Domain.Entities;
using StoreService.Domain.ValueObjects;

namespace StoreService.Infrastructure.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Primary Key
        builder.HasKey(s => s.Id);

        // Strongly Typed ID conversion
        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => StoreId.From(value))
            .IsRequired();

        // Properties
        builder.Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.LocationId)
            .IsRequired();

        // Address value object
        builder.OwnsOne(s => s.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Address_Street")
                .HasMaxLength(200)
                .IsRequired();
            
            address.Property(a => a.City)
                .HasColumnName("Address_City")
                .HasMaxLength(100)
                .IsRequired();
                
            address.Property(a => a.PostalCode)
                .HasColumnName("Address_PostalCode")
                .HasMaxLength(20)
                .IsRequired();
                
            address.Property(a => a.Country)
                .HasColumnName("Address_Country")
                .HasMaxLength(100)
                .IsRequired();
        });

        // Phone number value object
        builder.OwnsOne(s => s.Phone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("Phone")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.Property(s => s.OpeningHours)
            .HasMaxLength(100)
            .IsRequired();

        // Status enum conversion
        builder.Property(s => s.Status)
            .HasConversion(
                status => status.Name,
                name => StoreStatus.FromName(name))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        // Indexes
        builder.HasIndex(s => s.Name)
            .IsUnique();
        
        builder.HasIndex(s => s.LocationId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.CreatedAt);

        // Table configuration
        builder.ToTable("Stores", "store");
    }
} 