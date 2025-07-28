using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Repository;
using StoreService.Application.DTOs;
using StoreService.Domain.Entities;
using StoreService.Domain.ValueObjects;

namespace StoreService.Application.Queries;

public class GetStoreByIdQueryHandler : IQueryHandler<GetStoreByIdQuery, StoreDto?>
{
    private readonly IRepository<Store, StoreId> _storeRepository;

    public GetStoreByIdQueryHandler(IRepository<Store, StoreId> storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public async Task<StoreDto?> HandleAsync(GetStoreByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var store = await _storeRepository.GetByIdAsync(request.StoreId, cancellationToken);
        if (store is null)
            return null;

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