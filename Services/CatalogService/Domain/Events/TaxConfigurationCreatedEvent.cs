using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class TaxConfigurationCreatedEvent : DomainEventBase
{
    public TaxConfigId TaxConfigId { get; }
    public int LocationId { get; }
    public CategoryId? CategoryId { get; }
    public TaxRate TaxRate { get; }

    public TaxConfigurationCreatedEvent(TaxConfigId taxConfigId, int locationId, CategoryId? categoryId, TaxRate taxRate)
    {
        TaxConfigId = taxConfigId;
        LocationId = locationId;
        CategoryId = categoryId;
        TaxRate = taxRate;
    }
}