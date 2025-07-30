using BuildingBlocks.Application.CQRS.Queries;
using SharedService.Application.DTOs;

namespace SharedService.Application.Queries;

public class GetAllCurrenciesQuery : QueryBase<IEnumerable<CurrencyDto>>
{
} 