#pragma warning disable CA1812 // Validator is instantiated by FluentValidation
using FluentValidation;
using ReportingService.API.Endpoints;

namespace ReportingService.API.Validators;

internal sealed class CreatePromotionEffectivenessRequestValidator : AbstractValidator<CreatePromotionEffectivenessRequest>
{
    public CreatePromotionEffectivenessRequestValidator()
    {
        RuleFor(x => x.PromotionId)
            .NotEmpty().WithMessage("Promotion ID is required");

        RuleFor(x => x.RedemptionCount)
            .GreaterThanOrEqualTo(0).WithMessage("Redemption count cannot be negative");

        RuleFor(x => x.RevenueImpact)
            .NotNull().WithMessage("Revenue impact is required");

        RuleFor(x => x.AnalysisDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .When(x => x.AnalysisDate.HasValue)
            .WithMessage("Analysis date cannot be in the future");
    }
} 