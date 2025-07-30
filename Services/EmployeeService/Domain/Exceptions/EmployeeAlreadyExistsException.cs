using BuildingBlocks.Domain.Exceptions;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Domain.Exceptions;

public sealed class EmployeeAlreadyExistsException : DomainException
{
    public EmployeeAlreadyExistsException(EmployeeNumber employeeNumber)
        : base($"Employee with number '{employeeNumber}' already exists.")
    {
    }

    public EmployeeAlreadyExistsException(Guid userId)
        : base($"Employee with user ID '{userId}' already exists.")
    {
    }
}