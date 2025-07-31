using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BuildingBlocks.Domain.Common;
using ReportingService.Domain.Entities;
using ReportingService.Domain.ValueObjects;
using ReportingService.Infrastructure.Converters;

namespace ReportingService.Infrastructure.Configurations;

public class PromotionEffectivenessConfiguration : IEntityTypeConfiguration<PromotionEffectiveness>
{
    public void Configure(EntityTypeBuilder<PromotionEffectiveness> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("PromotionEffectiveness", "reporting");
        builder.HasKey(p => p.Id);

        // PromotionId
        builder.Property(p => p.PromotionId)
            .HasConversion(
                promotionId => promotionId.Value,
                value => PromotionId.From(value))
            .IsRequired();

        builder.HasIndex(p => p.PromotionId)
            .HasDatabaseName("IX_PromotionEffectiveness_PromotionId");

        // RedemptionCount
        builder.Property(p => p.RedemptionCount)
            .HasDefaultValue(0)
            .IsRequired();

        // RevenueImpact
        builder.Property(p => p.RevenueImpact)
            .HasColumnType("DECIMAL(19,4)")
            .IsRequired();

        // AnalysisDate
        builder.Property(p => p.AnalysisDate)
            .HasConversion<DateOnlyConverter>()
            .HasDefaultValueSql("CAST(GETDATE() AS DATE)")
            .IsRequired();

        builder.HasIndex(p => p.AnalysisDate)
            .HasDatabaseName("IX_PromotionEffectiveness_AnalysisDate");

        // Audit fields
        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(p => p.CreatedBy);

        // Additional indexes for performance
        builder.HasIndex(p => p.RedemptionCount)
            .HasDatabaseName("IX_PromotionEffectiveness_RedemptionCount");

        builder.HasIndex(p => p.RevenueImpact)
            .HasDatabaseName("IX_PromotionEffectiveness_RevenueImpact");

        builder.HasIndex(p => new { p.AnalysisDate, p.RedemptionCount })
            .HasDatabaseName("IX_PromotionEffectiveness_Date_Redemption");

        builder.HasIndex(p => new { p.AnalysisDate, p.RevenueImpact })
            .HasDatabaseName("IX_PromotionEffectiveness_Date_Revenue");
    }
} 