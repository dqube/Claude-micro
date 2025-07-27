using BuildingBlocks.Domain.Repository;
using ContactService.Domain.Entities;
using ContactService.Domain.ValueObjects;

namespace ContactService.Domain.Repositories;

public interface IContactRepository : IRepository<Contact, ContactId>
{
    Task<Contact?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Contact>> GetByContactTypeAsync(ContactType contactType, CancellationToken cancellationToken = default);
    Task<IEnumerable<Contact>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default);
}