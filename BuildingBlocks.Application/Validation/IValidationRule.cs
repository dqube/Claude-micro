namespace BuildingBlocks.Application.Validation;

public interface IValidationRule<in T>
{
    ValidationResult Validate(T instance);
    bool CanValidate(T instance);
}