using FluentValidation;
using PromotionService.Application.Commands;

namespace PromotionService.Application.Validators;

public class CreateDiscountCampaignCommandValidator : AbstractValidator<CreateDiscountCampaignCommand>
{
    public CreateDiscountCampaignCommandValidator()
    {
        // Ensure all validation rules are executed, not just the first failure per property
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Continue;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Campaign name is required")
            .Length(3, 100).WithMessage("Campaign name must be between 3 and 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Campaign description is required")
            .MaximumLength(500).WithMessage("Campaign description cannot exceed 500 characters");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Start date cannot be in the past");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");

        RuleFor(x => x.MaxUsesPerCustomer)
            .GreaterThan(0).When(x => x.MaxUsesPerCustomer.HasValue)
            .WithMessage("Max uses per customer must be greater than 0 when specified");

        RuleFor(x => x.IsActive)
            .NotNull().WithMessage("IsActive flag is required");
    }
} 