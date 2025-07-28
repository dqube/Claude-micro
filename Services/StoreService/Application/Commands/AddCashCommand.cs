using BuildingBlocks.Application.CQRS.Commands;
using StoreService.Domain.ValueObjects;

namespace StoreService.Application.Commands;

public class AddCashCommand : CommandBase
{
    public RegisterId RegisterId { get; init; }
    public decimal Amount { get; init; }
    public string Note { get; init; }

    public AddCashCommand(RegisterId registerId, decimal amount, string note)
    {
        RegisterId = registerId;
        Amount = amount;
        Note = note;
    }
} 