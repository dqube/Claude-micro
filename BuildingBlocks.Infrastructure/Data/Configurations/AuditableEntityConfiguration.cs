using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.StronglyTypedIds;

namespace BuildingBlocks.Infrastructure.Data.Configurations;

public abstract class AuditableEntityConfiguration<TEntity, TId> : EntityConfigurationBase<TEntity, TId>
    where TEntity : Entity<TId>, IAuditableEntity
    where TId : class, IStronglyTypedId
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(100);

        builder.Property(e => e.ModifiedAt);

        builder.Property(e => e.ModifiedBy)
            .HasMaxLength(100);

        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.ModifiedAt);
    }
}