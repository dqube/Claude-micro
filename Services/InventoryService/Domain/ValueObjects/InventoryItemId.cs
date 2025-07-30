using BuildingBlocks.Domain.StronglyTypedIds;

namespace InventoryService.Domain.ValueObjects;

public class InventoryItemId : StronglyTypedId<Guid>
{
    public InventoryItemId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("InventoryItemId cannot be empty", nameof(value));
    }
    
    public static InventoryItemId New() => new(Guid.NewGuid());
    
    public static InventoryItemId From(Guid value) => new(value);
}