using BuildingBlocks.Domain.Repository;
using SalesService.Domain.Entities;
using SalesService.Domain.ValueObjects;

namespace SalesService.Domain.Repositories;

public interface ISaleRepository : IRepository<Sale, SaleId>
{
    Task<Sale?> GetByReceiptNumberAsync(ReceiptNumber receiptNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByStoreIdAsync(int storeId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByEmployeeIdAsync(Guid employeeId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<Sale?> GetWithDetailsAsync(SaleId id, CancellationToken cancellationToken = default);
}