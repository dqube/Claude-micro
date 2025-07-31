#pragma warning disable CA1812 // Validator is instantiated by FluentValidation
using FluentValidation;
using ReportingService.API.Endpoints;

namespace ReportingService.API.Validators;

internal sealed class CreateInventorySnapshotRequestValidator : AbstractValidator<CreateInventorySnapshotRequest>
{
    public CreateInventorySnapshotRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.StoreId)
            .GreaterThan(0).WithMessage("Store ID must be a positive number");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative");

        RuleFor(x => x.SnapshotDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .When(x => x.SnapshotDate.HasValue)
            .WithMessage("Snapshot date cannot be in the future");
    }
} 