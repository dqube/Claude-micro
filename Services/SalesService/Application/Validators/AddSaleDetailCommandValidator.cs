using FluentValidation;
using SalesService.Application.Commands;

namespace SalesService.Application.Validators;

public class AddSaleDetailCommandValidator : AbstractValidator<AddSaleDetailCommand>
{
    public AddSaleDetailCommandValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Continue;

        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("Sale ID is required");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0");

        RuleFor(x => x.TaxApplied)
            .GreaterThanOrEqualTo(0).WithMessage("Tax applied cannot be negative");
    }
}