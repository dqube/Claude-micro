using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreService.Domain.Entities;
using StoreService.Domain.ValueObjects;

namespace StoreService.Infrastructure.Configurations;

public class RegisterConfiguration : IEntityTypeConfiguration<Register>
{
    public void Configure(EntityTypeBuilder<Register> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Primary Key
        builder.HasKey(r => r.Id);

        // Strongly Typed ID conversion
        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => RegisterId.From(value))
            .IsRequired();

        // Foreign Key to Store
        builder.Property(r => r.StoreId)
            .HasConversion(
                id => id.Value,
                value => StoreId.From(value))
            .IsRequired();

        // Properties
        builder.Property(r => r.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.CurrentBalance)
            .HasPrecision(19, 4)
            .IsRequired();

        // Status enum conversion
        builder.Property(r => r.Status)
            .HasConversion(
                status => status.Name,
                name => RegisterStatus.FromName(name))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.LastOpen);
        builder.Property(r => r.LastClose);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt);

        // Relationships
        builder.HasOne<Store>()
            .WithMany()
            .HasForeignKey(r => r.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(r => r.StoreId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => new { r.StoreId, r.Name })
            .IsUnique();

        // Table configuration
        builder.ToTable("Registers", "store");
    }
} 