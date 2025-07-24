using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("UserRoles", "auth");
        
        // Composite primary key (UserId, RoleId)
        builder.HasKey(ur => new { ur.Id, ur.RoleId });

        // Foreign key relationships
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(ur => ur.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Audit fields
        builder.Property(ur => ur.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(ur => ur.CreatedBy);

        // Indexes
        builder.HasIndex(ur => ur.Id)
            .HasDatabaseName("IX_UserRoles_UserId");

        builder.HasIndex(ur => ur.RoleId)
            .HasDatabaseName("IX_UserRoles_RoleId");

        builder.HasIndex(ur => ur.CreatedAt)
            .HasDatabaseName("IX_UserRoles_CreatedAt");
    }
}