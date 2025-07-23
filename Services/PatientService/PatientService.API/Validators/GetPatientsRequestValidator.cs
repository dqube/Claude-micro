using FluentValidation;
using PatientService.API.Endpoints;

namespace PatientService.API.Validators;

public class GetPatientsRequestValidator : AbstractValidator<GetPatientsRequest>
{
    public GetPatientsRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100).WithMessage("Search term cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        RuleFor(x => x.Gender)
            .Must(BeValidGender).WithMessage("Gender must be Male, Female, or Other")
            .When(x => !string.IsNullOrEmpty(x.Gender));

        RuleFor(x => x.MinAge)
            .InclusiveBetween(0, 150).WithMessage("Minimum age must be between 0 and 150")
            .When(x => x.MinAge.HasValue);

        RuleFor(x => x.MaxAge)
            .InclusiveBetween(0, 150).WithMessage("Maximum age must be between 0 and 150")
            .When(x => x.MaxAge.HasValue);

        RuleFor(x => x)
            .Must(x => !x.MinAge.HasValue || !x.MaxAge.HasValue || x.MinAge <= x.MaxAge)
            .WithMessage("Minimum age cannot be greater than maximum age")
            .When(x => x.MinAge.HasValue && x.MaxAge.HasValue);
    }

    private static bool BeValidGender(string gender)
    {
        var validGenders = new[] { "Male", "Female", "Other" };
        return validGenders.Contains(gender, StringComparer.OrdinalIgnoreCase);
    }
}