using BuildingBlocks.Domain.StronglyTypedIds;

namespace InventoryService.Domain.ValueObjects;

public class StoreId : StronglyTypedId<int>
{
    public StoreId(int value) : base(value)
    {
        if (value <= 0)
            throw new ArgumentException("StoreId must be greater than zero", nameof(value));
    }
    
    public static StoreId From(int value) => new(value);
}