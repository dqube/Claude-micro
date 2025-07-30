using BuildingBlocks.Application.CQRS.Commands;
using SharedService.Application.DTOs;

namespace SharedService.Application.Commands;

public class UpdateCurrencyCommand : CommandBase<CurrencyDto>
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;

    public UpdateCurrencyCommand(string code, string name, string symbol)
    {
        Code = code;
        Name = name;
        Symbol = symbol;
    }
} 