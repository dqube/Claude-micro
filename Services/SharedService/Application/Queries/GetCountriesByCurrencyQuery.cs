using BuildingBlocks.Application.CQRS.Queries;
using SharedService.Application.DTOs;

namespace SharedService.Application.Queries;

public class GetCountriesByCurrencyQuery : QueryBase<IEnumerable<CountryDto>>
{
    public string CurrencyCode { get; init; } = string.Empty;

    public GetCountriesByCurrencyQuery(string currencyCode)
    {
        CurrencyCode = currencyCode;
    }
} 