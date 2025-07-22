namespace BuildingBlocks.API.Validation.Results;

public class ValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<ValidationError> Errors { get; }

    private ValidationResult(bool isValid, IReadOnlyList<ValidationError> errors)
    {
        IsValid = isValid;
        Errors = errors;
    }

    public static ValidationResult Success()
    {
        return new ValidationResult(true, Array.Empty<ValidationError>());
    }

    public static ValidationResult Failure(params ValidationError[] errors)
    {
        return new ValidationResult(false, errors);
    }

    public static ValidationResult Failure(IEnumerable<ValidationError> errors)
    {
        return new ValidationResult(false, errors.ToArray());
    }

    public IDictionary<string, string[]> ToDictionary()
    {
        return Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray());
    }
}

public class ValidationError
{
    public string PropertyName { get; }
    public string ErrorMessage { get; }
    public object? AttemptedValue { get; }
    public string? ErrorCode { get; }

    public ValidationError(string propertyName, string errorMessage, object? attemptedValue = null, string? errorCode = null)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
        AttemptedValue = attemptedValue;
        ErrorCode = errorCode;
    }

    public static ValidationError Create(string propertyName, string errorMessage)
    {
        return new ValidationError(propertyName, errorMessage);
    }

    public static ValidationError Required(string propertyName)
    {
        return new ValidationError(propertyName, $"{propertyName} is required", errorCode: "REQUIRED");
    }

    public static ValidationError Invalid(string propertyName, object? value = null)
    {
        return new ValidationError(propertyName, $"{propertyName} is invalid", value, "INVALID");
    }

    public static ValidationError MaxLength(string propertyName, int maxLength)
    {
        return new ValidationError(propertyName, $"{propertyName} must not exceed {maxLength} characters", errorCode: "MAX_LENGTH");
    }

    public static ValidationError MinLength(string propertyName, int minLength)
    {
        return new ValidationError(propertyName, $"{propertyName} must be at least {minLength} characters", errorCode: "MIN_LENGTH");
    }
}