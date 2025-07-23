using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", "auth");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(v => v.Value, v => new RoleId(v))
            .ValueGeneratedNever(); // Assuming RoleId is manually assigned

        builder.Property(r => r.Name)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(r => r.Name)
            .IsUnique();

        builder.Property(r => r.Description)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(r => r.CreatedBy)
            .IsRequired(false);

        builder.Property(r => r.UpdatedAt)
            .IsRequired(false);

        builder.Property(r => r.UpdatedBy)
            .IsRequired(false);

        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId);
    }
}