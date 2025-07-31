#pragma warning disable CA1812 // Validator is instantiated by FluentValidation
using FluentValidation;
using SalesService.API.Endpoints;

namespace SalesService.API.Validators;

internal sealed class AddSaleDetailRequestValidator : AbstractValidator<AddSaleDetailRequest>
{
    public AddSaleDetailRequestValidator()
    {
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