#pragma warning disable CA1812 // Validator is instantiated by FluentValidation
using FluentValidation;

namespace AuthService.API.Validators;

// Placeholder for login request - would need corresponding endpoint and DTO
internal record LoginRequest(string Username, string Password);

internal sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MaximumLength(100).WithMessage("Username cannot exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters");
    }
}