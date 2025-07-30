using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromotionService.Domain.Entities;

namespace PromotionService.Infrastructure.Configurations;

public class DiscountCampaignConfiguration : IEntityTypeConfiguration<DiscountCampaign>
{
    public void Configure(EntityTypeBuilder<DiscountCampaign> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("DiscountCampaigns", "promotion");
        builder.HasKey(dc => dc.Id);

        // Name
        builder.Property(dc => dc.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(dc => dc.Name)
            .HasDatabaseName("IX_DiscountCampaigns_Name");

        // Description
        builder.Property(dc => dc.Description)
            .HasMaxLength(500);

        // Date fields
        builder.Property(dc => dc.StartDate)
            .IsRequired();

        builder.Property(dc => dc.EndDate)
            .IsRequired();

        // Status fields
        builder.Property(dc => dc.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(dc => dc.MaxUsesPerCustomer);

        // Audit fields
        builder.Property(dc => dc.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(dc => dc.CreatedBy);

        builder.Property(dc => dc.UpdatedAt);

        builder.Property(dc => dc.UpdatedBy);

        // Additional indexes
        builder.HasIndex(dc => dc.IsActive)
            .HasDatabaseName("IX_DiscountCampaigns_IsActive");

        builder.HasIndex(dc => dc.StartDate)
            .HasDatabaseName("IX_DiscountCampaigns_StartDate");

        builder.HasIndex(dc => dc.EndDate)
            .HasDatabaseName("IX_DiscountCampaigns_EndDate");

        builder.HasIndex(dc => dc.CreatedAt)
            .HasDatabaseName("IX_DiscountCampaigns_CreatedAt");

        // Configure relationships
        builder.HasMany(dc => dc.Rules)
            .WithOne()
            .HasForeignKey(dr => dr.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 