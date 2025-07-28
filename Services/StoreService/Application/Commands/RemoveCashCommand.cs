using BuildingBlocks.Application.CQRS.Commands;
using StoreService.Domain.ValueObjects;

namespace StoreService.Application.Commands;

public class RemoveCashCommand : CommandBase
{
    public RegisterId RegisterId { get; init; }
    public decimal Amount { get; init; }
    public string Note { get; init; }

    public RemoveCashCommand(RegisterId registerId, decimal amount, string note)
    {
        RegisterId = registerId;
        Amount = amount;
        Note = note;
    }
} 