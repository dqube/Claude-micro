using BuildingBlocks.Application.CQRS.Commands;
using ContactService.Application.DTOs;

namespace ContactService.Application.Commands;

public class UpdateContactCommand : CommandBase
{
    public Guid ContactId { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public AddressDto? Address { get; init; }
    public string? Company { get; init; }
    public string? JobTitle { get; init; }
    public string? Notes { get; init; }

    public UpdateContactCommand(
        Guid contactId,
        string firstName,
        string lastName,
        string email,
        string? phoneNumber = null,
        AddressDto? address = null,
        string? company = null,
        string? jobTitle = null,
        string? notes = null)
    {
        ContactId = contactId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        Company = company;
        JobTitle = jobTitle;
        Notes = notes;
    }
}