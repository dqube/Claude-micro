using BuildingBlocks.Application.CQRS.Queries;
using SharedService.Application.DTOs;

namespace SharedService.Application.Queries;

public class GetCountryByCodeQuery : QueryBase<CountryDto?>
{
    public string Code { get; init; } = string.Empty;

    public GetCountryByCodeQuery(string code)
    {
        Code = code;
    }
} 