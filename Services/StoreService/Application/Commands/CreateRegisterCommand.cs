using BuildingBlocks.Application.CQRS.Commands;
using StoreService.Application.DTOs;
using StoreService.Domain.ValueObjects;

namespace StoreService.Application.Commands;

public class CreateRegisterCommand : CommandBase<RegisterDto>
{
    public StoreId StoreId { get; init; }
    public string Name { get; init; }

    public CreateRegisterCommand(StoreId storeId, string name)
    {
        StoreId = storeId;
        Name = name;
    }
} 