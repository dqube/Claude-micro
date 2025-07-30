using BuildingBlocks.Domain.StronglyTypedIds;

namespace InventoryService.Domain.ValueObjects;

public class StockMovementId : StronglyTypedId<Guid>
{
    public StockMovementId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("StockMovementId cannot be empty", nameof(value));
    }
    
    public static StockMovementId New() => new(Guid.NewGuid());
    
    public static StockMovementId From(Guid value) => new(value);
}