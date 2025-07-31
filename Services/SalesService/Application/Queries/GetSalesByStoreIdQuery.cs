using BuildingBlocks.Application.CQRS.Queries;
using SalesService.Application.DTOs;

namespace SalesService.Application.Queries;

public class GetSalesByStoreIdQuery : QueryBase<IEnumerable<SaleDto>>
{
    public int StoreId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }

    public GetSalesByStoreIdQuery(int storeId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        StoreId = storeId;
        FromDate = fromDate;
        ToDate = toDate;
    }
}