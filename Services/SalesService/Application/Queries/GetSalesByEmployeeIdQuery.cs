using BuildingBlocks.Application.CQRS.Queries;
using SalesService.Application.DTOs;

namespace SalesService.Application.Queries;

public class GetSalesByEmployeeIdQuery : QueryBase<IEnumerable<SaleDto>>
{
    public Guid EmployeeId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }

    public GetSalesByEmployeeIdQuery(Guid employeeId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        EmployeeId = employeeId;
        FromDate = fromDate;
        ToDate = toDate;
    }
}