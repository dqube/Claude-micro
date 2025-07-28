using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Repository;
using BuildingBlocks.Domain.ValueObjects;
using StoreService.Application.DTOs;
using StoreService.Domain.Entities;
using StoreService.Domain.ValueObjects;

namespace StoreService.Application.Commands;

public class CreateStoreCommandHandler : ICommandHandler<CreateStoreCommand, StoreDto>
{
    private readonly IRepository<Store, StoreId> _storeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStoreCommandHandler(
        IRepository<Store, StoreId> storeRepository,
        IUnitOfWork unitOfWork)
    {
        _storeRepository = storeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StoreDto> HandleAsync(CreateStoreCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var storeId = StoreId.From(0); // Will be set by database identity
        var address = new Address(
            request.Address.Street,
            request.Address.City,
            request.Address.PostalCode,
            request.Address.Country);
        var phone = new PhoneNumber(request.PhoneNumber);

        var store = new Store(
            storeId,
            request.Name,
            request.LocationId,
            address,
            phone,
            request.OpeningHours);

        await _storeRepository.AddAsync(store, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(store);
    }

    private static StoreDto MapToDto(Store store)
    {
        return new StoreDto
        {
            Id = store.Id.Value,
            Name = store.Name,
            LocationId = store.LocationId,
            Address = new AddressDto(
                store.Address.Street,
                store.Address.City,
                store.Address.PostalCode,
                store.Address.Country),
            PhoneNumber = store.Phone.Value,
            OpeningHours = store.OpeningHours,
            Status = store.Status.Name,
            IsOperational = store.IsOperational,
            CreatedAt = store.CreatedAt,
            UpdatedAt = store.UpdatedAt
        };
    }
} 