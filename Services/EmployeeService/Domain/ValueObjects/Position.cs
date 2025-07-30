using BuildingBlocks.Domain.ValueObjects;

namespace EmployeeService.Domain.ValueObjects;

public sealed class Position : SingleValueObject<string>
{
    public Position(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Position cannot be null or empty", nameof(value));
        if (value.Length > 50)
            throw new ArgumentException("Position cannot exceed 50 characters", nameof(value));
    }

    public static implicit operator string(Position position) => position.Value;
    public static implicit operator Position(string value) => new(value);
}