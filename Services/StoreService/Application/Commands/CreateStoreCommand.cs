using BuildingBlocks.Application.CQRS.Commands;
using StoreService.Application.DTOs;

namespace StoreService.Application.Commands;

public class CreateStoreCommand : CommandBase<StoreDto>
{
    public string Name { get; init; }
    public int LocationId { get; init; }
    public AddressDto Address { get; init; }
    public string PhoneNumber { get; init; }
    public string OpeningHours { get; init; }

    public CreateStoreCommand(
        string name,
        int locationId,
        AddressDto address,
        string phoneNumber,
        string openingHours)
    {
        Name = name;
        LocationId = locationId;
        Address = address;
        PhoneNumber = phoneNumber;
        OpeningHours = openingHours;
    }
} 