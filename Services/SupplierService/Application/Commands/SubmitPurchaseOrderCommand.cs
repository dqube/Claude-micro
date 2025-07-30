using BuildingBlocks.Application.CQRS.Commands;
using SupplierService.Application.DTOs;

namespace SupplierService.Application.Commands;

public class SubmitPurchaseOrderCommand : CommandBase<PurchaseOrderDto>
{
    public Guid Id { get; init; }

    public SubmitPurchaseOrderCommand(Guid id)
    {
        Id = id;
    }
} 