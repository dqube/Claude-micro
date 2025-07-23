using FluentValidation;
using PatientService.API.Endpoints;

namespace PatientService.API.Validators;

public class UpdateContactRequestValidator : AbstractValidator<UpdateContactRequest>
{
    public UpdateContactRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Phone number format is invalid")
            .Length(10, 20).WithMessage("Phone number must be between 10 and 20 characters")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}