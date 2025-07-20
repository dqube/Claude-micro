namespace BuildingBlocks.Application.Validation;

public interface IValidator<in T>
{
    Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
    ValidationResult Validate(T instance);
}