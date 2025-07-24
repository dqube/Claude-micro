using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("Roles", "auth");
        builder.HasKey(r => r.Id);

        // Name
        builder.Property(r => r.Name)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasDatabaseName("IX_Roles_Name");

        // Description
        builder.Property(r => r.Description)
            .HasMaxLength(255);

        // Audit fields
        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(r => r.CreatedBy);

        builder.Property(r => r.UpdatedAt);

        builder.Property(r => r.UpdatedBy);

        // Index for created date
        builder.HasIndex(r => r.CreatedAt)
            .HasDatabaseName("IX_Roles_CreatedAt");
    }
}