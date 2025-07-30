using BuildingBlocks.Application.CQRS.Queries;
using SharedService.Application.DTOs;

namespace SharedService.Application.Queries;

public class GetCurrencyByCodeQuery : QueryBase<CurrencyDto?>
{
    public string Code { get; init; } = string.Empty;

    public GetCurrencyByCodeQuery(string code)
    {
        Code = code;
    }
} 