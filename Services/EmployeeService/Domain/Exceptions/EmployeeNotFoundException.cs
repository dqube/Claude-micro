using BuildingBlocks.Domain.Exceptions;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Domain.Exceptions;

public sealed class EmployeeNotFoundException : DomainException
{
    public EmployeeNotFoundException(EmployeeId employeeId)
        : base($"Employee with ID '{employeeId}' was not found.")
    {
    }

    public EmployeeNotFoundException(EmployeeNumber employeeNumber)
        : base($"Employee with number '{employeeNumber}' was not found.")
    {
    }

    public EmployeeNotFoundException(Guid userId)
        : base($"Employee with user ID '{userId}' was not found.")
    {
    }
}