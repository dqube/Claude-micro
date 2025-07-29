using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class TaxConfigurationCategoryUpdatedEvent : DomainEventBase
{
    public TaxConfigId TaxConfigId { get; }
    public int LocationId { get; }
    public CategoryId? OldCategoryId { get; }
    public CategoryId? NewCategoryId { get; }

    public TaxConfigurationCategoryUpdatedEvent(TaxConfigId taxConfigId, int locationId, CategoryId? oldCategoryId, CategoryId? newCategoryId)
    {
        TaxConfigId = taxConfigId;
        LocationId = locationId;
        OldCategoryId = oldCategoryId;
        NewCategoryId = newCategoryId;
    }
}