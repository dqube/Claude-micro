using FluentValidation;
using SalesService.Application.Commands;

namespace SalesService.Application.Validators;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Continue;

        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required");

        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Employee ID is required");

        RuleFor(x => x.RegisterId)
            .NotEmpty().WithMessage("Register ID is required")
            .GreaterThan(0).WithMessage("Register ID must be greater than 0");
    }
}