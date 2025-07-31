using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesService.Domain.Entities;

namespace SalesService.Infrastructure.Configurations;

public class AppliedDiscountConfiguration : IEntityTypeConfiguration<AppliedDiscount>
{
    public void Configure(EntityTypeBuilder<AppliedDiscount> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("AppliedDiscounts", "sales");
        builder.HasKey(ad => ad.Id);

        builder.Property(ad => ad.SaleDetailId);

        builder.Property(ad => ad.SaleId);

        builder.Property(ad => ad.CampaignId)
            .IsRequired();

        builder.Property(ad => ad.RuleId)
            .IsRequired();

        builder.Property(ad => ad.DiscountAmount)
            .HasColumnType("DECIMAL(19,4)")
            .IsRequired();

        builder.Property(ad => ad.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(ad => ad.CreatedBy);

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_AppliedDiscounts_SaleOrDetail",
            "(SaleDetailId IS NOT NULL AND SaleId IS NULL) OR (SaleDetailId IS NULL AND SaleId IS NOT NULL)"));

        builder.HasIndex(ad => ad.SaleDetailId)
            .HasDatabaseName("IX_AppliedDiscounts_SaleDetail")
            .HasFilter("[SaleDetailId] IS NOT NULL");

        builder.HasIndex(ad => ad.SaleId)
            .HasDatabaseName("IX_AppliedDiscounts_Sale")
            .HasFilter("[SaleId] IS NOT NULL");

        builder.HasIndex(ad => ad.CampaignId)
            .HasDatabaseName("IX_AppliedDiscounts_Campaign");
    }
}