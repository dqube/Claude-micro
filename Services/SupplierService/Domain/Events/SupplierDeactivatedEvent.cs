using BuildingBlocks.Domain.DomainEvents;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Events;

public class SupplierDeactivatedEvent : DomainEventBase
{
    public SupplierId SupplierId { get; }
    public string SupplierName { get; }

    public SupplierDeactivatedEvent(SupplierId supplierId, string supplierName)
    {
        SupplierId = supplierId ?? throw new ArgumentNullException(nameof(supplierId));
        SupplierName = supplierName ?? throw new ArgumentNullException(nameof(supplierName));
    }
} 