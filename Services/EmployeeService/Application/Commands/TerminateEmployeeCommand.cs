using BuildingBlocks.Application.CQRS.Commands;

namespace EmployeeService.Application.Commands;

public class TerminateEmployeeCommand : CommandBase
{
    public Guid EmployeeId { get; init; }
    public DateTime TerminationDate { get; init; }

    public TerminateEmployeeCommand(Guid employeeId, DateTime terminationDate)
    {
        EmployeeId = employeeId;
        TerminationDate = terminationDate;
    }
}