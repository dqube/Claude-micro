using BuildingBlocks.Application.CQRS.Queries;
using ContactService.Application.DTOs;
using ContactService.Domain.Repositories;
using ContactService.Domain.ValueObjects;

namespace ContactService.Application.Queries;

public class GetContactByIdQueryHandler : IQueryHandler<GetContactByIdQuery, ContactDto?>
{
    private readonly IContactRepository _contactRepository;

    public GetContactByIdQueryHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<ContactDto?> HandleAsync(GetContactByIdQuery query, CancellationToken cancellationToken)
    {
        var contactId = ContactId.From(query.ContactId);
        var contact = await _contactRepository.GetByIdAsync(contactId, cancellationToken);

        if (contact == null)
            return null;

        return new ContactDto
        {
            Id = contact.Id.Value,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            FullName = contact.FullName,
            Email = contact.Email.Value,
            PhoneNumber = contact.PhoneNumber?.Value,
            Address = contact.Address != null ? new AddressDto
            {
                Street = contact.Address.Street,
                City = contact.Address.City,
                PostalCode = contact.Address.PostalCode,
                Country = contact.Address.Country,
                FullAddress = contact.Address.FullAddress
            } : null,
            ContactType = contact.ContactType.Name,
            Company = contact.Company,
            JobTitle = contact.JobTitle,
            Notes = contact.Notes,
            IsActive = contact.IsActive
        };
    }
}