using BuildingBlocks.Application.CQRS.Queries;
using SalesService.Application.DTOs;

namespace SalesService.Application.Queries;

public class GetReturnsBySaleIdQuery : QueryBase<IEnumerable<ReturnDto>>
{
    public Guid SaleId { get; init; }

    public GetReturnsBySaleIdQuery(Guid saleId)
    {
        SaleId = saleId;
    }
}