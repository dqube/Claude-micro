using BuildingBlocks.Application.CQRS.Commands;

namespace EmployeeService.Application.Commands;

public class CreateEmployeeCommand : CommandBase<Guid>
{
    public Guid UserId { get; init; }
    public int StoreId { get; init; }
    public string EmployeeNumber { get; init; } = string.Empty;
    public DateTime HireDate { get; init; }
    public string Position { get; init; } = string.Empty;
    public int AuthLevel { get; init; } = 1;

    public CreateEmployeeCommand(Guid userId, int storeId, string employeeNumber, DateTime hireDate, string position, int authLevel = 1)
    {
        UserId = userId;
        StoreId = storeId;
        EmployeeNumber = employeeNumber;
        HireDate = hireDate;
        Position = position;
        AuthLevel = authLevel;
    }
}