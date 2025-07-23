using FluentValidation;
using PatientService.API.Endpoints;
using System.Text.RegularExpressions;

namespace PatientService.API.Validators;

public class CreatePatientRequestValidator : AbstractValidator<CreatePatientRequest>
{
    public CreatePatientRequestValidator()
    {
        RuleFor(x => x.MedicalRecordNumber)
            .NotEmpty().WithMessage("Medical record number is required")
            .Length(5, 20).WithMessage("Medical record number must be between 5 and 20 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters")
            .Matches("^[a-zA-Z\\s'-]+$").WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters")
            .Matches("^[a-zA-Z\\s'-]+$").WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");

        RuleFor(x => x.MiddleName)
            .MaximumLength(50).WithMessage("Middle name cannot exceed 50 characters")
            .Matches("^[a-zA-Z\\s'-]*$").WithMessage("Middle name can only contain letters, spaces, hyphens, and apostrophes")
            .When(x => !string.IsNullOrEmpty(x.MiddleName));

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Phone number format is invalid")
            .Length(10, 20).WithMessage("Phone number must be between 10 and 20 characters")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
            .GreaterThan(DateTime.Today.AddYears(-150)).WithMessage("Date of birth cannot be more than 150 years ago");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required")
            .Must(BeValidGender).WithMessage("Gender must be Male, Female, or Other");

        RuleFor(x => x.BloodType)
            .Must(BeValidBloodType).WithMessage("Blood type must be A+, A-, B+, B-, AB+, AB-, O+, or O-")
            .When(x => !string.IsNullOrEmpty(x.BloodType));

        When(x => x.Address != null, () =>
        {
            RuleFor(x => x.Address!.Street)
                .NotEmpty().WithMessage("Street address is required when address is provided")
                .MaximumLength(100).WithMessage("Street address cannot exceed 100 characters");

            RuleFor(x => x.Address!.City)
                .NotEmpty().WithMessage("City is required when address is provided")
                .MaximumLength(50).WithMessage("City cannot exceed 50 characters");

            RuleFor(x => x.Address!.PostalCode)
                .NotEmpty().WithMessage("Postal code is required when address is provided")
                .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters");

            RuleFor(x => x.Address!.Country)
                .NotEmpty().WithMessage("Country is required when address is provided")
                .MaximumLength(50).WithMessage("Country cannot exceed 50 characters");
        });
    }

    private static bool BeValidGender(string gender)
    {
        var validGenders = new[] { "Male", "Female", "Other" };
        return validGenders.Contains(gender, StringComparer.OrdinalIgnoreCase);
    }

    private static bool BeValidBloodType(string? bloodType)
    {
        if (string.IsNullOrEmpty(bloodType))
            return true;

        var validBloodTypes = new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
        return validBloodTypes.Contains(bloodType, StringComparer.OrdinalIgnoreCase);
    }
}