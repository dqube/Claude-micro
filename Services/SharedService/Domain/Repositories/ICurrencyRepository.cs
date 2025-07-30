using BuildingBlocks.Domain.Repository;
using SharedService.Domain.Entities;
using SharedService.Domain.ValueObjects;

namespace SharedService.Domain.Repositories;

public interface ICurrencyRepository : IRepository<Currency, CurrencyCode>, IReadOnlyRepository<Currency, CurrencyCode>
{
    Task<Currency?> GetByCodeAsync(CurrencyCode code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Currency>> GetAllActiveAsync(CancellationToken cancellationToken = default);
} 