using FluentValidation;
using SalesService.Application.Commands;

namespace SalesService.Application.Validators;

public class AddReturnDetailCommandValidator : AbstractValidator<AddReturnDetailCommand>
{
    public AddReturnDetailCommandValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Continue;

        RuleFor(x => x.ReturnId)
            .NotEmpty().WithMessage("Return ID is required");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Return reason is required")
            .MaximumLength(500).WithMessage("Return reason cannot exceed 500 characters");
    }
}