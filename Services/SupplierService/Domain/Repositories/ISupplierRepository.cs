using BuildingBlocks.Domain.Repository;
using SupplierService.Domain.Entities;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Repositories;

public interface ISupplierRepository : IRepository<Supplier, SupplierId>, IReadOnlyRepository<Supplier, SupplierId>
{
    Task<Supplier?> GetByIdWithContactsAsync(SupplierId id, CancellationToken cancellationToken = default);
    Task<Supplier?> GetByIdWithAddressesAsync(SupplierId id, CancellationToken cancellationToken = default);
    Task<Supplier?> GetByIdWithPurchaseOrdersAsync(SupplierId id, CancellationToken cancellationToken = default);
    Task<Supplier?> GetCompleteAsync(SupplierId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Supplier>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Supplier>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Supplier>> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken = default);
} 