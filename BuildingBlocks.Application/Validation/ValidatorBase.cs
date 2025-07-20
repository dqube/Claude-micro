namespace BuildingBlocks.Application.Validation;

public abstract class ValidatorBase<T> : IValidator<T>
{
    private readonly List<IValidationRule<T>> _rules = [];

    protected void AddRule(IValidationRule<T> rule)
    {
        _rules.Add(rule);
    }

    public virtual ValidationResult Validate(T instance)
    {
        var result = new ValidationResult();
        
        foreach (var rule in _rules.Where(r => r.CanValidate(instance)))
        {
            var ruleResult = rule.Validate(instance);
            if (!ruleResult.IsValid)
            {
                foreach (var error in ruleResult.Errors)
                {
                    result.AddError(error);
                }
            }
        }

        return result;
    }

    public virtual Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Validate(instance));
    }
}