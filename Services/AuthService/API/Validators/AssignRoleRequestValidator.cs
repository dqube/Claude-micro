#pragma warning disable CA1812 // Validator is instantiated by FluentValidation
using FluentValidation;
using AuthService.API.Endpoints;

namespace AuthService.API.Validators;

internal sealed class AssignRoleRequestValidator : AbstractValidator<AssignRoleRequest>
{
    public AssignRoleRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("Role ID must be a positive integer");

        RuleFor(x => x.AssignedBy)
            .NotEmpty().WithMessage("AssignedBy user ID is required");
    }
}