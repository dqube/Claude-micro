namespace BuildingBlocks.Application.Validation;

public class ValidationResult
{
    private readonly List<ValidationError> _errors = [];

    public ValidationResult()
    {
    }

    public ValidationResult(IEnumerable<ValidationError> errors)
    {
        _errors.AddRange(errors);
    }

    public bool IsValid => !_errors.Any();
    public IReadOnlyList<ValidationError> Errors => _errors.AsReadOnly();

    public void AddError(ValidationError error)
    {
        _errors.Add(error);
    }

    public void AddError(string propertyName, string errorMessage)
    {
        _errors.Add(new ValidationError(propertyName, errorMessage));
    }

    public static ValidationResult Success() => new();
    public static ValidationResult Failure(params ValidationError[] errors) => new(errors);
}

public class ValidationError
{
    public ValidationError(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }

    public string PropertyName { get; }
    public string ErrorMessage { get; }
}