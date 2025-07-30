using BuildingBlocks.Application.CQRS.Commands;

namespace SharedService.Application.Commands;

public class DeleteCountryCommand : CommandBase<bool>
{
    public string Code { get; init; } = string.Empty;

    public DeleteCountryCommand(string code)
    {
        Code = code;
    }
} 