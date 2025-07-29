using BuildingBlocks.Domain.StronglyTypedIds;

namespace StoreService.Domain.ValueObjects;

public class StoreId : StronglyTypedId<int>
{
    public StoreId(int value) : base(value)
    {
        if (value < 0)
            throw new ArgumentException("StoreId must be a positive integer", nameof(value));
    }
    
    public static StoreId From(int value) => new(value);
} 