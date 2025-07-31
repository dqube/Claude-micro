using BuildingBlocks.Application.CQRS.Queries;
using SalesService.Application.DTOs;

namespace SalesService.Application.Queries;

public class GetSalesByCustomerIdQuery : QueryBase<IEnumerable<SaleDto>>
{
    public Guid CustomerId { get; init; }

    public GetSalesByCustomerIdQuery(Guid customerId)
    {
        CustomerId = customerId;
    }
}