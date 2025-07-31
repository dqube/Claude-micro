using BuildingBlocks.Application.CQRS.Queries;
using SalesService.Application.DTOs;

namespace SalesService.Application.Queries;

public class GetSaleByIdQuery : QueryBase<SaleDto?>
{
    public Guid SaleId { get; init; }

    public GetSaleByIdQuery(Guid saleId)
    {
        SaleId = saleId;
    }
}