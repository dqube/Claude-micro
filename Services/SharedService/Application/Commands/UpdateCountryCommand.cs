using BuildingBlocks.Application.CQRS.Commands;
using SharedService.Application.DTOs;

namespace SharedService.Application.Commands;

public class UpdateCountryCommand : CommandBase<CountryDto>
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string CurrencyCode { get; init; } = string.Empty;

    public UpdateCountryCommand(string code, string name, string currencyCode)
    {
        Code = code;
        Name = name;
        CurrencyCode = currencyCode;
    }
} 