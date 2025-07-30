using FluentValidation;
using EmployeeService.API.Endpoints;

namespace EmployeeService.API.Validators;

public sealed class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");

        RuleFor(x => x.StoreId)
            .GreaterThan(0)
            .WithMessage("StoreId must be greater than 0");

        RuleFor(x => x.EmployeeNumber)
            .NotEmpty()
            .MaximumLength(20)
            .WithMessage("EmployeeNumber is required and must not exceed 20 characters");

        RuleFor(x => x.Position)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Position is required and must not exceed 50 characters");

        RuleFor(x => x.AuthLevel)
            .InclusiveBetween(1, 10)
            .WithMessage("AuthLevel must be between 1 and 10");

        RuleFor(x => x.HireDate)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("HireDate cannot be in the future");
    }
}