using BuildingBlocks.Application.CQRS.Queries;
using SalesService.Application.DTOs;

namespace SalesService.Application.Queries;

public class GetSalesByDateRangeQuery : QueryBase<IEnumerable<SaleDto>>
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }

    public GetSalesByDateRangeQuery(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }
}