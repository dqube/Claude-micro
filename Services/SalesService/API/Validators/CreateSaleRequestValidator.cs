#pragma warning disable CA1812 // Validator is instantiated by FluentValidation
using FluentValidation;
using SalesService.API.Endpoints;

namespace SalesService.API.Validators;

internal sealed class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required");

        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Employee ID is required");

        RuleFor(x => x.RegisterId)
            .NotEmpty().WithMessage("Register ID is required")
            .GreaterThan(0).WithMessage("Register ID must be greater than 0");
    }
}