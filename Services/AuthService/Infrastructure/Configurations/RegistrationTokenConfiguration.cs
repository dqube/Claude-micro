using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BuildingBlocks.Domain.Common;
using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;

namespace AuthService.Infrastructure.Configurations;

public class RegistrationTokenConfiguration : IEntityTypeConfiguration<RegistrationToken>
{
    public void Configure(EntityTypeBuilder<RegistrationToken> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("RegistrationTokens", "auth", t => 
            t.HasCheckConstraint("CK_RegistrationTokens_TokenType", 
                "[TokenType] IN ('EmailVerification', 'PasswordReset')"));
        builder.HasKey(rt => rt.Id);

        // Token
        builder.Property(rt => rt.Token)
            .HasMaxLength(100)
            .IsRequired();

        // Email
        builder.Property(rt => rt.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .HasMaxLength(100)
            .IsRequired();

        // UserId (foreign key)
        builder.Property(rt => rt.UserId)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // TokenType
        builder.Property(rt => rt.TokenType)
            .HasConversion(
                tokenType => tokenType.Value,
                value => TokenType.From(value))
            .HasMaxLength(20)
            .IsRequired();

        // Expiration
        builder.Property(rt => rt.Expiration)
            .HasDefaultValueSql("DATEADD(HOUR, 24, GETDATE())")
            .IsRequired();

        // Usage tracking
        builder.Property(rt => rt.IsUsed)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(rt => rt.UsedAt);

        builder.Property(rt => rt.UsedBy);

        // Audit fields
        builder.Property(rt => rt.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(rt => rt.CreatedBy);

        // Indexes
        builder.HasIndex(rt => rt.Token)
            .IsUnique()
            .HasDatabaseName("IX_RegistrationTokens_Token");

        builder.HasIndex(rt => rt.UserId)
            .HasDatabaseName("IX_RegistrationTokens_UserId");

        builder.HasIndex(rt => rt.Email)
            .HasDatabaseName("IX_RegistrationTokens_Email");

        builder.HasIndex(rt => rt.Expiration)
            .HasDatabaseName("IX_RegistrationTokens_Expiration")
            .HasFilter("[IsUsed] = 0");

        builder.HasIndex(rt => new { rt.TokenType, rt.IsUsed })
            .HasDatabaseName("IX_RegistrationTokens_TokenType_IsUsed");

        builder.HasIndex(rt => rt.CreatedAt)
            .HasDatabaseName("IX_RegistrationTokens_CreatedAt");
    }
}