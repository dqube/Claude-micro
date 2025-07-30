using BuildingBlocks.Application.CQRS.Queries;
using EmployeeService.Application.DTOs;

namespace EmployeeService.Application.Queries;

public class GetEmployeeByIdQuery : QueryBase<EmployeeDto>
{
    public Guid EmployeeId { get; init; }

    public GetEmployeeByIdQuery(Guid employeeId)
    {
        EmployeeId = employeeId;
    }
}