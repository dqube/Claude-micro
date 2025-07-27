using BuildingBlocks.Application.CQRS.Commands;
using ContactService.Application.DTOs;

namespace ContactService.Application.Commands;

public class CreateContactCommand : CommandBase
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public AddressDto? Address { get; init; }
    public string ContactType { get; init; }
    public string? Company { get; init; }
    public string? JobTitle { get; init; }
    public string? Notes { get; init; }

    public CreateContactCommand(
        string firstName,
        string lastName,
        string email,
        string contactType,
        string? phoneNumber = null,
        AddressDto? address = null,
        string? company = null,
        string? jobTitle = null,
        string? notes = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        ContactType = contactType;
        PhoneNumber = phoneNumber;
        Address = address;
        Company = company;
        JobTitle = jobTitle;
        Notes = notes;
    }
}