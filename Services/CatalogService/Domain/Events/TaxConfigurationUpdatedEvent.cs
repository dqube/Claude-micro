using BuildingBlocks.Domain.DomainEvents;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Events;

public class TaxConfigurationUpdatedEvent : DomainEventBase
{
    public TaxConfigId TaxConfigId { get; }
    public int LocationId { get; }
    public CategoryId? CategoryId { get; }
    public TaxRate OldTaxRate { get; }
    public TaxRate NewTaxRate { get; }

    public TaxConfigurationUpdatedEvent(TaxConfigId taxConfigId, int locationId, CategoryId? categoryId, TaxRate oldTaxRate, TaxRate newTaxRate)
    {
        TaxConfigId = taxConfigId;
        LocationId = locationId;
        CategoryId = categoryId;
        OldTaxRate = oldTaxRate;
        NewTaxRate = newTaxRate;
    }
}