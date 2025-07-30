using BuildingBlocks.Domain.Repository;
using SupplierService.Domain.Entities;
using SupplierService.Domain.ValueObjects;

namespace SupplierService.Domain.Repositories;

public interface ISupplierAddressRepository : IRepository<SupplierAddress, AddressId>, IReadOnlyRepository<SupplierAddress, AddressId>
{
    Task<IEnumerable<SupplierAddress>> GetBySupplierIdAsync(SupplierId supplierId, CancellationToken cancellationToken = default);
    Task<SupplierAddress?> GetPrimaryAddressAsync(SupplierId supplierId, CancellationToken cancellationToken = default);
    Task<SupplierAddress?> GetShippingAddressAsync(SupplierId supplierId, CancellationToken cancellationToken = default);
    Task<SupplierAddress?> GetBillingAddressAsync(SupplierId supplierId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplierAddress>> GetByCountryAsync(string countryCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplierAddress>> GetByCityAsync(string city, CancellationToken cancellationToken = default);
} 