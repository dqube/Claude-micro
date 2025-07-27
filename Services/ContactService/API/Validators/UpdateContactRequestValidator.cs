#pragma warning disable CA1812 // Validator is instantiated by FluentValidation
using FluentValidation;
using ContactService.API.Endpoints;

namespace ContactService.API.Validators;

internal sealed class UpdateContactRequestValidator : AbstractValidator<UpdateContactRequest>
{
    public UpdateContactRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
            .Matches("^[a-zA-Z\\s'-]+$").WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters")
            .Matches("^[a-zA-Z\\s'-]+$").WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address")
            .MaximumLength(254).WithMessage("Email cannot exceed 254 characters");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^[\+]?[0-9\-\(\)\s]{7,20}$").WithMessage("Phone number format is invalid")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Company)
            .MaximumLength(200).WithMessage("Company cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Company));

        RuleFor(x => x.JobTitle)
            .MaximumLength(100).WithMessage("Job title cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.JobTitle));

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));

        When(x => x.Address != null, () =>
        {
            RuleFor(x => x.Address!.Street)
                .NotEmpty().WithMessage("Street address is required when address is provided")
                .MaximumLength(200).WithMessage("Street address cannot exceed 200 characters");

            RuleFor(x => x.Address!.City)
                .NotEmpty().WithMessage("City is required when address is provided")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

            RuleFor(x => x.Address!.PostalCode)
                .NotEmpty().WithMessage("Postal code is required when address is provided")
                .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters");

            RuleFor(x => x.Address!.Country)
                .NotEmpty().WithMessage("Country is required when address is provided")
                .MaximumLength(100).WithMessage("Country cannot exceed 100 characters");
        });
    }
}