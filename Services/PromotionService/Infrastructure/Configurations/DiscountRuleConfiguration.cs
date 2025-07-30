using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromotionService.Domain.Entities;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Infrastructure.Configurations;

public class DiscountRuleConfiguration : IEntityTypeConfiguration<DiscountRule>
{
    public void Configure(EntityTypeBuilder<DiscountRule> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("DiscountRules", "promotion");
        builder.HasKey(dr => dr.Id);

        // Campaign ID (Foreign Key)
        builder.Property(dr => dr.CampaignId)
            .IsRequired();

        // Rule Type
        builder.Property(dr => dr.RuleType)
            .HasConversion(
                ruleType => ruleType.Value,
                value => RuleType.From(value))
            .HasMaxLength(50)
            .IsRequired();

        // Product and Category targeting
        builder.Property(dr => dr.ProductId);
        builder.Property(dr => dr.CategoryId);

        // Minimum requirements
        builder.Property(dr => dr.MinQuantity);
        builder.Property(dr => dr.MinAmount)
            .HasColumnType("decimal(18,2)");

        // Discount configuration
        builder.Property(dr => dr.DiscountValue)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(dr => dr.DiscountMethod)
            .HasConversion(
                discountMethod => discountMethod.Value,
                value => DiscountMethod.From(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(dr => dr.FreeProductId);

        // Audit fields
        builder.Property(dr => dr.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(dr => dr.CreatedBy);

        builder.Property(dr => dr.UpdatedAt);

        builder.Property(dr => dr.UpdatedBy);

        // Additional indexes
        builder.HasIndex(dr => dr.CampaignId)
            .HasDatabaseName("IX_DiscountRules_CampaignId");

        builder.HasIndex(dr => dr.RuleType)
            .HasDatabaseName("IX_DiscountRules_RuleType");

        builder.HasIndex(dr => dr.ProductId)
            .HasDatabaseName("IX_DiscountRules_ProductId")
            .HasFilter("[ProductId] IS NOT NULL");

        builder.HasIndex(dr => dr.CategoryId)
            .HasDatabaseName("IX_DiscountRules_CategoryId")
            .HasFilter("[CategoryId] IS NOT NULL");

        builder.HasIndex(dr => dr.CreatedAt)
            .HasDatabaseName("IX_DiscountRules_CreatedAt");
    }
} 