namespace BuildingBlocks.API.Validation.Results;

public class ValidationResult
{
    public bool IsValid { get; init; }
    public IReadOnlyCollection<ValidationError> Errors { get; init; } = Array.Empty<ValidationError>();
    public string? ErrorMessage => IsValid ? null : string.Join("; ", Errors.Select(e => e.ErrorMessage));

    public static ValidationResult Success() => new() { IsValid = true };

    public static ValidationResult Failure(params ValidationError[] errors) => new()
    {
        IsValid = false,
        Errors = errors
    };

    public static ValidationResult Failure(IEnumerable<ValidationError> errors) => new()
    {
        IsValid = false,
        Errors = errors.ToArray()
    };

    public static ValidationResult Failure(string propertyName, string errorMessage) => new()
    {
        IsValid = false,
        Errors = new[] { new ValidationError(propertyName, errorMessage) }
    };

    public ValidationResult Combine(ValidationResult other)
    {
        if (IsValid && other.IsValid)
            return Success();

        var allErrors = Errors.Concat(other.Errors).ToArray();
        return Failure(allErrors);
    }

    public static ValidationResult operator +(ValidationResult left, ValidationResult right)
    {
        return left.Combine(right);
    }

    public Dictionary<string, string[]> ToDictionary()
    {
        return Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );
    }
}