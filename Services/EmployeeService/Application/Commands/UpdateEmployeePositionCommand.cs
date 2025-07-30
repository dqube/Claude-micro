using BuildingBlocks.Application.CQRS.Commands;

namespace EmployeeService.Application.Commands;

public class UpdateEmployeePositionCommand : CommandBase
{
    public Guid EmployeeId { get; init; }
    public string Position { get; init; } = string.Empty;

    public UpdateEmployeePositionCommand(Guid employeeId, string position)
    {
        EmployeeId = employeeId;
        Position = position;
    }
}