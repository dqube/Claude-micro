namespace BuildingBlocks.API.Validation.Results;

public record ValidationError(string PropertyName, string ErrorMessage)
{
    public string? AttemptedValue { get; init; }
    public string? ErrorCode { get; init; }
    public object? CustomState { get; init; }

    public static ValidationError Create(string propertyName, string errorMessage, string? errorCode = null)
    {
        return new ValidationError(propertyName, errorMessage)
        {
            ErrorCode = errorCode
        };
    }

    public static ValidationError Create(string propertyName, string errorMessage, object? attemptedValue, string? errorCode = null)
    {
        return new ValidationError(propertyName, errorMessage)
        {
            AttemptedValue = attemptedValue?.ToString(),
            ErrorCode = errorCode
        };
    }

    public ValidationError WithAttemptedValue(object? value)
    {
        return this with { AttemptedValue = value?.ToString() };
    }

    public ValidationError WithErrorCode(string errorCode)
    {
        return this with { ErrorCode = errorCode };
    }

    public ValidationError WithCustomState(object customState)
    {
        return this with { CustomState = customState };
    }

    public override string ToString()
    {
        return $"{PropertyName}: {ErrorMessage}";
    }
}