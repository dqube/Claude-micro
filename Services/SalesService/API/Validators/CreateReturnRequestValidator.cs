#pragma warning disable CA1812 // Validator is instantiated by FluentValidation
using FluentValidation;
using SalesService.API.Endpoints;

namespace SalesService.API.Validators;

internal sealed class CreateReturnRequestValidator : AbstractValidator<CreateReturnRequest>
{
    public CreateReturnRequestValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("Sale ID is required");

        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Employee ID is required");
    }
}