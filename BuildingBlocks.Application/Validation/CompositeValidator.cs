namespace BuildingBlocks.Application.Validation;

public class CompositeValidator<T> : IValidator<T>
{
    private readonly List<IValidator<T>> _validators = [];

    public CompositeValidator(params IValidator<T>[] validators)
    {
        _validators.AddRange(validators);
    }

    public void AddValidator(IValidator<T> validator)
    {
        _validators.Add(validator);
    }

    public ValidationResult Validate(T instance)
    {
        var result = new ValidationResult();
        
        foreach (var validator in _validators)
        {
            var validationResult = validator.Validate(instance);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    result.AddError(error);
                }
            }
        }

        return result;
    }

    public async Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default)
    {
        var result = new ValidationResult();
        
        foreach (var validator in _validators)
        {
            var validationResult = await validator.ValidateAsync(instance, cancellationToken);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    result.AddError(error);
                }
            }
        }

        return result;
    }
}