using FluentValidation;
using PromotionService.Application.Commands;

namespace PromotionService.Application.Validators;

public class CreatePromotionCommandValidator : AbstractValidator<CreatePromotionCommand>
{
    public CreatePromotionCommandValidator()
    {
        // Ensure all validation rules are executed, not just the first failure per property
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Continue;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Promotion name is required")
            .Length(3, 100).WithMessage("Promotion name must be between 3 and 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Promotion description is required")
            .MaximumLength(500).WithMessage("Promotion description cannot exceed 500 characters");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Start date cannot be in the past");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");

        RuleFor(x => x.MaxRedemptions)
            .GreaterThan(0).When(x => x.MaxRedemptions.HasValue)
            .WithMessage("Max redemptions must be greater than 0 when specified");

        RuleFor(x => x.IsCombinable)
            .NotNull().WithMessage("IsCombinable flag is required");
    }
} 