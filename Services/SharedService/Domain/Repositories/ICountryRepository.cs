using BuildingBlocks.Domain.Repository;
using SharedService.Domain.Entities;
using SharedService.Domain.ValueObjects;

namespace SharedService.Domain.Repositories;

public interface ICountryRepository : IRepository<Country, CountryCode>, IReadOnlyRepository<Country, CountryCode>
{
    Task<Country?> GetByCodeAsync(CountryCode code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Country>> GetByCurrencyAsync(CurrencyCode currencyCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<Country>> GetAllActiveAsync(CancellationToken cancellationToken = default);
} 