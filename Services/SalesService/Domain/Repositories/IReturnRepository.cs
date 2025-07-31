using BuildingBlocks.Domain.Repository;
using SalesService.Domain.Entities;
using SalesService.Domain.ValueObjects;

namespace SalesService.Domain.Repositories;

public interface IReturnRepository : IRepository<Return, ReturnId>
{
    Task<IEnumerable<Return>> GetBySaleIdAsync(SaleId saleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Return>> GetByEmployeeIdAsync(Guid employeeId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Return>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Return?> GetWithDetailsAsync(ReturnId id, CancellationToken cancellationToken = default);
}