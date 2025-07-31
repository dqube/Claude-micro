#pragma warning disable CA1812 // Validator is instantiated by FluentValidation
using FluentValidation;
using SalesService.API.Endpoints;

namespace SalesService.API.Validators;

internal sealed class AddReturnDetailRequestValidator : AbstractValidator<AddReturnDetailRequest>
{
    public AddReturnDetailRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Return reason is required")
            .MaximumLength(500).WithMessage("Return reason cannot exceed 500 characters");
    }
}