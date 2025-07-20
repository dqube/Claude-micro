using FluentValidation;

namespace BuildingBlocks.API.Validation.Validators;

public class PaginationRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";
}

public class PaginationValidator : AbstractValidator<PaginationRequest>
{
    public PaginationValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.SortDirection)
            .Must(x => string.IsNullOrEmpty(x) || x.ToLower() is "asc" or "desc")
            .WithMessage("SortDirection must be 'asc' or 'desc'");
    }
}