using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class ProductBasicInfoUpdatedEvent : DomainEventBase
{
    public ProductId ProductId { get; }
    public string OldName { get; }
    public string NewName { get; }
    public string? OldDescription { get; }
    public string? NewDescription { get; }

    public ProductBasicInfoUpdatedEvent(ProductId productId, string oldName, string newName, string? oldDescription, string? newDescription)
    {
        ProductId = productId;
        OldName = oldName;
        NewName = newName;
        OldDescription = oldDescription;
        NewDescription = newDescription;
    }
}