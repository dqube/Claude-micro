using FluentValidation;
using SalesService.Application.Commands;

namespace SalesService.Application.Validators;

public class ApplyDiscountCommandValidator : AbstractValidator<ApplyDiscountCommand>
{
    public ApplyDiscountCommandValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Continue;

        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("Sale ID is required");

        RuleFor(x => x.CampaignId)
            .NotEmpty().WithMessage("Campaign ID is required");

        RuleFor(x => x.RuleId)
            .NotEmpty().WithMessage("Rule ID is required");

        RuleFor(x => x.DiscountAmount)
            .GreaterThan(0).WithMessage("Discount amount must be greater than 0");
    }
}