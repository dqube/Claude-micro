using BuildingBlocks.Domain.ValueObjects;

namespace EmployeeService.Domain.ValueObjects;

public sealed class EmployeeNumber : SingleValueObject<string>
{
    public EmployeeNumber(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("EmployeeNumber cannot be null or empty", nameof(value));
        if (value.Length > 20)
            throw new ArgumentException("EmployeeNumber cannot exceed 20 characters", nameof(value));
    }

    public static implicit operator string(EmployeeNumber employeeNumber) => employeeNumber.Value;
    public static implicit operator EmployeeNumber(string value) => new(value);
}