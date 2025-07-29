using BuildingBlocks.Domain.StronglyTypedIds;

namespace CatalogService.Domain.ValueObjects;

public class CategoryId : StronglyTypedId<int>
{
    public CategoryId(int value) : base(value)
    {
        if (value <= 0)
            throw new ArgumentException("CategoryId must be positive", nameof(value));
    }
    
    public static CategoryId From(int value) => new(value);
}