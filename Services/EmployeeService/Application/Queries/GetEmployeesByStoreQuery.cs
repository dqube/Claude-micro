using BuildingBlocks.Application.CQRS.Queries;
using EmployeeService.Application.DTOs;

namespace EmployeeService.Application.Queries;

public class GetEmployeesByStoreQuery : QueryBase<IEnumerable<EmployeeDto>>
{
    public int StoreId { get; init; }

    public GetEmployeesByStoreQuery(int storeId)
    {
        StoreId = storeId;
    }
}