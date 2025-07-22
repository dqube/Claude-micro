using FluentValidation;

namespace BuildingBlocks.API.Validation.Validators;

public abstract class RequestValidator<T> : AbstractValidator<T>
{
    protected void ValidateId(System.Linq.Expressions.Expression<Func<T, int>> idSelector, string fieldName = "Id")
    {
        RuleFor(idSelector)
            .GreaterThan(0)
            .WithMessage($"{fieldName} must be greater than 0");
    }

    protected void ValidateGuidId(System.Linq.Expressions.Expression<Func<T, Guid>> idSelector, string fieldName = "Id")
    {
        RuleFor(idSelector)
            .NotEmpty()
            .WithMessage($"{fieldName} must be a valid GUID");
    }
}