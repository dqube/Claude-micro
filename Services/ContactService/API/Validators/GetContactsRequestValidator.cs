#pragma warning disable CA1812 // Validator is instantiated by FluentValidation
using FluentValidation;
using ContactService.API.Endpoints;

namespace ContactService.API.Validators;

internal sealed class GetContactsRequestValidator : AbstractValidator<GetContactsRequest>
{
    public GetContactsRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200).WithMessage("Search term cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        RuleFor(x => x.ContactType)
            .Must(BeValidContactType).WithMessage("Contact type must be Personal, Business, or Emergency")
            .When(x => !string.IsNullOrEmpty(x.ContactType));

        RuleFor(x => x.SortBy)
            .Must(BeValidSortField).WithMessage("Sort field must be FirstName, LastName, Email, or CreatedAt")
            .When(x => !string.IsNullOrEmpty(x.SortBy));
    }

    private static bool BeValidContactType(string? contactType)
    {
        if (string.IsNullOrEmpty(contactType))
            return true;

        var validTypes = new[] { "Personal", "Business", "Emergency" };
        return validTypes.Contains(contactType, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidSortField(string? sortField)
    {
        if (string.IsNullOrEmpty(sortField))
            return true;

        var validFields = new[] { "FirstName", "LastName", "Email", "CreatedAt" };
        return validFields.Contains(sortField, StringComparer.OrdinalIgnoreCase);
    }
}