using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CatalogService.Domain.Entities;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Infrastructure.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("ProductCategories", "catalog");

        builder.HasKey(pc => pc.Id);

        builder.Property(pc => pc.Id)
            .HasConversion(
                id => id.Value,
                value => CategoryId.From(value))
            .HasColumnName("CategoryId")
            .ValueGeneratedNever(); // We handle ID generation manually

        builder.Property(pc => pc.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(pc => pc.Name)
            .IsUnique()
            .HasDatabaseName("IX_ProductCategories_Name");

        builder.Property(pc => pc.ParentCategoryId)
            .HasConversion(
                id => id != null ? id.Value : (int?)null,
                value => value.HasValue ? CategoryId.From(value.Value) : null);

        builder.Property(pc => pc.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(pc => pc.CreatedBy);

        builder.Property(pc => pc.UpdatedAt);

        builder.Property(pc => pc.UpdatedBy);

        // Self-referencing relationship
        builder.HasOne<ProductCategory>()
            .WithMany()
            .HasForeignKey(pc => pc.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}