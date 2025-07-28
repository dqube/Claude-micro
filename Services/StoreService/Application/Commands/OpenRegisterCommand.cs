using BuildingBlocks.Application.CQRS.Commands;
using StoreService.Domain.ValueObjects;

namespace StoreService.Application.Commands;

public class OpenRegisterCommand : CommandBase
{
    public RegisterId RegisterId { get; init; }
    public decimal StartingCash { get; init; }

    public OpenRegisterCommand(RegisterId registerId, decimal startingCash)
    {
        RegisterId = registerId;
        StartingCash = startingCash;
    }
} 