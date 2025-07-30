using BuildingBlocks.Domain.Repository;
using SupplierService.Domain.Entities;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Repositories;

public interface ISupplierContactRepository : IRepository<SupplierContact, ContactId>, IReadOnlyRepository<SupplierContact, ContactId>
{
    Task<IEnumerable<SupplierContact>> GetBySupplierIdAsync(SupplierId supplierId, CancellationToken cancellationToken = default);
    Task<SupplierContact?> GetPrimaryContactAsync(SupplierId supplierId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplierContact>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<SupplierContact?> GetWithContactNumbersAsync(ContactId id, CancellationToken cancellationToken = default);
} 