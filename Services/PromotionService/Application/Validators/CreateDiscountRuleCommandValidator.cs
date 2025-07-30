using FluentValidation;
using PromotionService.Application.Commands;

namespace PromotionService.Application.Validators;

public class CreateDiscountRuleCommandValidator : AbstractValidator<CreateDiscountRuleCommand>
{
    public CreateDiscountRuleCommandValidator()
    {
        // Ensure all validation rules are executed, not just the first failure per property
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Continue;

        RuleFor(x => x.CampaignId)
            .NotEmpty().WithMessage("Campaign ID is required");

        RuleFor(x => x.RuleType)
            .NotEmpty().WithMessage("Rule type is required")
            .Must(BeValidRuleType).WithMessage("Invalid rule type");

        RuleFor(x => x.DiscountValue)
            .GreaterThan(0).WithMessage("Discount value must be greater than 0");

        RuleFor(x => x.DiscountMethod)
            .NotEmpty().WithMessage("Discount method is required")
            .Must(BeValidDiscountMethod).WithMessage("Invalid discount method");

        RuleFor(x => x.MinQuantity)
            .GreaterThan(0).When(x => x.MinQuantity.HasValue)
            .WithMessage("Minimum quantity must be greater than 0 when specified");

        RuleFor(x => x.MinAmount)
            .GreaterThan(0).When(x => x.MinAmount.HasValue)
            .WithMessage("Minimum amount must be greater than 0 when specified");

        // Business rule: Either ProductId or CategoryId should be specified for product/category rules
        RuleFor(x => x)
            .Must(HaveValidTargetSpecification)
            .WithMessage("Either ProductId or CategoryId must be specified for the rule type");
    }

    private static bool BeValidRuleType(string ruleType)
    {
        var validTypes = new[] { "Product", "Category", "Cart", "Buy X Get Y" };
        return validTypes.Contains(ruleType);
    }

    private static bool BeValidDiscountMethod(string discountMethod)
    {
        var validMethods = new[] { "Percentage", "FixedAmount", "FreeProduct" };
        return validMethods.Contains(discountMethod);
    }

    private static bool HaveValidTargetSpecification(CreateDiscountRuleCommand command)
    {
        // For product-specific rules, ProductId should be specified
        if (command.RuleType == "Product" && !command.ProductId.HasValue)
            return false;

        // For category-specific rules, CategoryId should be specified
        if (command.RuleType == "Category" && !command.CategoryId.HasValue)
            return false;

        return true;
    }
} 