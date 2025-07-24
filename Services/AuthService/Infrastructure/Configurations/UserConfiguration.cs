using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BuildingBlocks.Domain.Common;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;

namespace AuthService.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("Users", "auth");
        builder.HasKey(u => u.Id);

        // Username
        builder.Property(u => u.Username)
            .HasConversion(
                username => username.Value,
                value => new Username(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(u => u.Username)
            .IsUnique()
            .HasDatabaseName("IX_Users_Username");

        // Email
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        // Password Hash
        builder.Property(u => u.PasswordHash)
            .HasConversion(
                hash => hash.Value,
                value => new PasswordHash(value))
            .IsRequired();

        // Password Salt
        builder.Property(u => u.PasswordSalt)
            .HasColumnType("VARBINARY(MAX)")
            .IsRequired();

        // Status and security fields
        builder.Property(u => u.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(u => u.FailedLoginAttempts)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(u => u.LockoutEnd);

        // Audit fields
        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(u => u.CreatedBy);

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.UpdatedBy);

        // Additional indexes
        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_Users_IsActive");

        builder.HasIndex(u => u.LockoutEnd)
            .HasDatabaseName("IX_Users_LockoutEnd")
            .HasFilter("[LockoutEnd] IS NOT NULL");

        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("IX_Users_CreatedAt");
    }
}