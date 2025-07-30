using BuildingBlocks.Application.CQRS.Queries;
using SupplierService.Application.DTOs;

namespace SupplierService.Application.Queries;

public class GetPurchaseOrderByIdQuery : QueryBase<PurchaseOrderDto?>
{
    public Guid Id { get; init; }
    public bool IncludeDetails { get; init; }

    public GetPurchaseOrderByIdQuery(Guid id, bool includeDetails = true)
    {
        Id = id;
        IncludeDetails = includeDetails;
    }
} 