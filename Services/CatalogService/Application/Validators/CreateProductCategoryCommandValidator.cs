using FluentValidation;
using CatalogService.Application.Commands;

namespace CatalogService.Application.Validators;

public class CreateProductCategoryCommandValidator : AbstractValidator<CreateProductCategoryCommand>
{
    public CreateProductCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required")
            .MaximumLength(100)
            .WithMessage("Category name cannot exceed 100 characters");

        RuleFor(x => x.ParentCategoryId)
            .GreaterThan(0)
            .WithMessage("Parent category ID must be positive")
            .When(x => x.ParentCategoryId.HasValue);
    }
}