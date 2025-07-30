using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromotionService.Domain.Entities;

namespace PromotionService.Infrastructure.Configurations;

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("Promotions", "promotion");
        builder.HasKey(p => p.Id);

        // Name
        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_Promotions_Name");

        // Description
        builder.Property(p => p.Description)
            .HasMaxLength(500);

        // Date fields
        builder.Property(p => p.StartDate)
            .IsRequired();

        builder.Property(p => p.EndDate)
            .IsRequired();

        // Configuration fields
        builder.Property(p => p.IsCombinable)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(p => p.MaxRedemptions);

        // Audit fields
        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(p => p.CreatedBy);

        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.UpdatedBy);

        // Additional indexes
        builder.HasIndex(p => p.IsCombinable)
            .HasDatabaseName("IX_Promotions_IsCombinable");

        builder.HasIndex(p => p.StartDate)
            .HasDatabaseName("IX_Promotions_StartDate");

        builder.HasIndex(p => p.EndDate)
            .HasDatabaseName("IX_Promotions_EndDate");

        builder.HasIndex(p => p.CreatedAt)
            .HasDatabaseName("IX_Promotions_CreatedAt");

        // Configure relationships
        builder.HasMany(p => p.PromotionProducts)
            .WithOne()
            .HasForeignKey(pp => pp.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 