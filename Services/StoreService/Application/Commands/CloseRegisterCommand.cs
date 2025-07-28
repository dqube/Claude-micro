using BuildingBlocks.Application.CQRS.Commands;
using StoreService.Domain.ValueObjects;

namespace StoreService.Application.Commands;

public class CloseRegisterCommand : CommandBase
{
    public RegisterId RegisterId { get; init; }
    public decimal EndingCash { get; init; }

    public CloseRegisterCommand(RegisterId registerId, decimal endingCash)
    {
        RegisterId = registerId;
        EndingCash = endingCash;
    }
} 