using BuildingBlocks.Domain.Repository;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Domain.Repositories;

public interface IPaymentProcessorRepository : IRepository<PaymentProcessor, PaymentProcessorId>, IReadOnlyRepository<PaymentProcessor, PaymentProcessorId>
{
    /// <summary>
    /// Gets all active payment processors
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of active payment processors</returns>
    Task<IEnumerable<PaymentProcessor>> GetActiveProcessorsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets payment processor by name
    /// </summary>
    /// <param name="name">Processor name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment processor if found</returns>
    Task<PaymentProcessor?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if processor name exists
    /// </summary>
    /// <param name="name">Processor name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if name exists</returns>
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
} 