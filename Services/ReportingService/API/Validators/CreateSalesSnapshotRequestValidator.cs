#pragma warning disable CA1812 // Validator is instantiated by FluentValidation
using FluentValidation;
using ReportingService.API.Endpoints;

namespace ReportingService.API.Validators;

internal sealed class CreateSalesSnapshotRequestValidator : AbstractValidator<CreateSalesSnapshotRequest>
{
    public CreateSalesSnapshotRequestValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("Sale ID is required");

        RuleFor(x => x.StoreId)
            .GreaterThan(0).WithMessage("Store ID must be a positive number");

        RuleFor(x => x.SaleDate)
            .NotEmpty().WithMessage("Sale date is required")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today)).WithMessage("Sale date cannot be in the future");

        RuleFor(x => x.TotalAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Total amount cannot be negative");
    }
} 