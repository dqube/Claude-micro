using BuildingBlocks.Domain.Entities;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Entities;

public class DiscountType : Entity<DiscountTypeId>
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    // Private constructor for EF Core
    private DiscountType() : base(DiscountTypeId.From(0))
    {
        Name = string.Empty;
    }

    public DiscountType(DiscountTypeId id, string name, string? description = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Discount type name cannot be null or empty", nameof(name));
        
        if (name.Length > 50)
            throw new ArgumentException("Discount type name cannot exceed 50 characters", nameof(name));

        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string? description, Guid updatedBy)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
} 