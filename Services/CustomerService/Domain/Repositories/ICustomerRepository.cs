using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.Common;
using CustomerService.Domain.Entities;
using CustomerService.Domain.ValueObjects;

namespace CustomerService.Domain.Repositories;

public interface ICustomerRepository : IRepository<Customer, CustomerId>
{
    Task<Customer?> GetByMembershipNumberAsync(MembershipNumber membershipNumber, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> MembershipNumberExistsAsync(MembershipNumber membershipNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetCustomersWithExpiredMembershipsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetCustomersByCountryAsync(string countryCode, CancellationToken cancellationToken = default);
    Task<Customer?> GetCustomerWithContactsAndAddressesAsync(CustomerId customerId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Customer> Customers, int TotalCount)> GetCustomersPagedAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null, 
        string? countryCode = null, 
        bool? isMembershipActive = null, 
        CancellationToken cancellationToken = default);
} 