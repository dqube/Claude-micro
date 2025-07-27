using BuildingBlocks.Application.CQRS.Queries;
using ContactService.Application.DTOs;
using ContactService.Domain.Repositories;

namespace ContactService.Application.Queries;

public class GetContactsQueryHandler : IQueryHandler<GetContactsQuery, PagedResult<ContactDto>>
{
    private readonly IContactRepository _contactRepository;

    public GetContactsQueryHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<PagedResult<ContactDto>> HandleAsync(GetContactsQuery query, CancellationToken cancellationToken)
    {
        // This is a simplified implementation. In a real scenario, you'd implement proper paging in the repository
        var allContacts = await _contactRepository.GetAllAsync(cancellationToken);
        
        var filteredContacts = allContacts.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            filteredContacts = filteredContacts.Where(c => 
                c.FirstName.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                c.LastName.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Value.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (c.Company != null && c.Company.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(query.ContactType))
        {
            filteredContacts = filteredContacts.Where(c => 
                c.ContactType.Name.Equals(query.ContactType, StringComparison.OrdinalIgnoreCase));
        }

        if (query.IsActive.HasValue)
        {
            filteredContacts = filteredContacts.Where(c => c.IsActive == query.IsActive.Value);
        }

        var totalCount = filteredContacts.Count();
        var pagedContacts = filteredContacts
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(contact => new ContactDto
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
            })
            .ToList();

        return new PagedResult<ContactDto>(pagedContacts, totalCount, query.Page, query.PageSize);
    }
}