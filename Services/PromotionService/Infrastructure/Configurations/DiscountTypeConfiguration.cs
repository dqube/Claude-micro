using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromotionService.Domain.Entities;

namespace PromotionService.Infrastructure.Configurations;

public class DiscountTypeConfiguration : IEntityTypeConfiguration<DiscountType>
{
    public void Configure(EntityTypeBuilder<DiscountType> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("DiscountTypes", "promotion");
        builder.HasKey(dt => dt.Id);

        // Name
        builder.Property(dt => dt.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(dt => dt.Name)
            .IsUnique()
            .HasDatabaseName("IX_DiscountTypes_Name");

        // Description
        builder.Property(dt => dt.Description)
            .HasMaxLength(500);

        // Audit fields
        builder.Property(dt => dt.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(dt => dt.CreatedBy);

        builder.Property(dt => dt.UpdatedAt);

        builder.Property(dt => dt.UpdatedBy);

        // Additional indexes
        builder.HasIndex(dt => dt.CreatedAt)
            .HasDatabaseName("IX_DiscountTypes_CreatedAt");
    }
} 