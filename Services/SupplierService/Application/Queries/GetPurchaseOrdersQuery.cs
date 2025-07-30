using BuildingBlocks.Application.CQRS.Queries;
using SupplierService.Application.DTOs;

namespace SupplierService.Application.Queries;

public class GetPurchaseOrdersQuery : QueryBase<IEnumerable<PurchaseOrderDto>>
{
    public Guid? SupplierId { get; init; }
    public int? StoreId { get; init; }
    public string? Status { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }

    public GetPurchaseOrdersQuery(Guid? supplierId = null, int? storeId = null, string? status = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        SupplierId = supplierId;
        StoreId = storeId;
        Status = status;
        FromDate = fromDate;
        ToDate = toDate;
    }
} 