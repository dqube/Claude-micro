using FluentValidation;
using InventoryService.API.Endpoints;

namespace InventoryService.API.Validators;

internal class CreateStockMovementRequestValidator : AbstractValidator<CreateStockMovementRequest>
{
    public CreateStockMovementRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.StoreId)
            .GreaterThan(0)
            .WithMessage("Store ID must be greater than zero");

        RuleFor(x => x.MovementType)
            .NotEmpty()
            .WithMessage("Movement type is required")
            .Must(BeValidMovementType)
            .WithMessage("Movement type must be one of: Purchase, Return, Adjustment, Damage, Transfer");

        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("Employee ID is required");
    }

    private static bool BeValidMovementType(string movementType)
    {
        if (string.IsNullOrEmpty(movementType))
            return false;

        var validTypes = new[] { "Purchase", "Return", "Adjustment", "Damage", "Transfer" };
        return validTypes.Contains(movementType, StringComparer.OrdinalIgnoreCase);
    }
}