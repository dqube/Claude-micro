using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromotionService.Domain.Entities;

namespace PromotionService.Infrastructure.Configurations;

public class PromotionProductConfiguration : IEntityTypeConfiguration<PromotionProduct>
{
    public void Configure(EntityTypeBuilder<PromotionProduct> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("PromotionProducts", "promotion");
        builder.HasKey(pp => pp.Id);

        // Promotion ID (Foreign Key)
        builder.Property(pp => pp.PromotionId)
            .IsRequired();

        // Product and Category targeting
        builder.Property(pp => pp.ProductId);
        builder.Property(pp => pp.CategoryId);

        // Minimum quantity
        builder.Property(pp => pp.MinQuantity)
            .HasDefaultValue(1)
            .IsRequired();

        // Discount configuration
        builder.Property(pp => pp.DiscountPercent)
            .HasColumnType("decimal(5,2)");

        builder.Property(pp => pp.BundlePrice)
            .HasColumnType("decimal(18,2)");

        // Audit fields
        builder.Property(pp => pp.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        // Additional indexes
        builder.HasIndex(pp => pp.PromotionId)
            .HasDatabaseName("IX_PromotionProducts_PromotionId");

        builder.HasIndex(pp => pp.ProductId)
            .HasDatabaseName("IX_PromotionProducts_ProductId")
            .HasFilter("[ProductId] IS NOT NULL");

        builder.HasIndex(pp => pp.CategoryId)
            .HasDatabaseName("IX_PromotionProducts_CategoryId")
            .HasFilter("[CategoryId] IS NOT NULL");

        builder.HasIndex(pp => pp.CreatedAt)
            .HasDatabaseName("IX_PromotionProducts_CreatedAt");

        // Unique constraint to prevent duplicate product/category entries per promotion
        builder.HasIndex(new[] { nameof(PromotionProduct.PromotionId), nameof(PromotionProduct.ProductId) })
            .IsUnique()
            .HasDatabaseName("IX_PromotionProducts_PromotionId_ProductId")
            .HasFilter("[ProductId] IS NOT NULL");

        builder.HasIndex(new[] { nameof(PromotionProduct.PromotionId), nameof(PromotionProduct.CategoryId) })
            .IsUnique()
            .HasDatabaseName("IX_PromotionProducts_PromotionId_CategoryId")
            .HasFilter("[CategoryId] IS NOT NULL");
    }
} 