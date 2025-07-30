using BuildingBlocks.Application.CQRS.Commands;

namespace SharedService.Application.Commands;

public class DeleteCurrencyCommand : CommandBase<bool>
{
    public string Code { get; init; } = string.Empty;

    public DeleteCurrencyCommand(string code)
    {
        Code = code;
    }
} 