using BuildingBlocks.Domain.Repository;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Domain.Repositories;

public interface IEmployeeRepository : IRepository<Employee, EmployeeId>
{
    Task<Employee?> GetByEmployeeNumberAsync(EmployeeNumber employeeNumber, CancellationToken cancellationToken = default);
    Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetByStoreIdAsync(int storeId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmployeeNumberAsync(EmployeeNumber employeeNumber, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}