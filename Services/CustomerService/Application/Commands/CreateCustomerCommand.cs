using BuildingBlocks.Application.CQRS.Commands;
using CustomerService.Application.DTOs;

namespace CustomerService.Application.Commands;

public class CreateCustomerCommand : CommandBase<CustomerDto>
{
    public Guid? UserId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? MembershipNumber { get; init; }
    public DateTime? JoinDate { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public string CountryCode { get; init; } = string.Empty;

    public CreateCustomerCommand(
        string firstName,
        string lastName,
        string countryCode,
        Guid? userId = null,
        string? email = null,
        string? membershipNumber = null,
        DateTime? joinDate = null,
        DateTime? expiryDate = null)
    {
        FirstName = firstName;
        LastName = lastName;
        CountryCode = countryCode;
        UserId = userId;
        Email = email;
        MembershipNumber = membershipNumber;
        JoinDate = joinDate ?? DateTime.UtcNow;
        ExpiryDate = expiryDate ?? DateTime.UtcNow.AddYears(1); // Default 1 year membership
    }
} 