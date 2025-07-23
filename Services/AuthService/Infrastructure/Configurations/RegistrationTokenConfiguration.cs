using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations;

public class RegistrationTokenConfiguration : IEntityTypeConfiguration<RegistrationToken>
{
    public void Configure(EntityTypeBuilder<RegistrationToken> builder)
    {
        builder.ToTable("RegistrationTokens", "auth");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Id)
            .HasConversion(v => v.Value, v => new RegistrationTokenId(v))
            .ValueGeneratedOnAdd();

        builder.Property(rt => rt.TokenType)
            .HasConversion(v => v.Value, v => new TokenType(v))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(rt => rt.Expiration)
            .IsRequired();

        builder.Property(rt => rt.IsUsed)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(rt => rt.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(rt => rt.CreatedBy)
            .IsRequired(false);

        builder.HasIndex(rt => rt.Expiration)
            .HasFilter("[IsUsed] = 0");
    }
}