using BuildingBlocks.Application.CQRS.Queries;
using BuildingBlocks.Domain.Repository;
using StoreService.Application.DTOs;
using StoreService.Domain.Entities;
using StoreService.Domain.ValueObjects;

namespace StoreService.Application.Queries;

public class GetStoresQueryHandler : IQueryHandler<GetStoresQuery, PagedResult<StoreDto>>
{
    private readonly IRepository<Store, StoreId> _storeRepository;

    public GetStoresQueryHandler(IRepository<Store, StoreId> storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public async Task<PagedResult<StoreDto>> HandleAsync(GetStoresQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var storesQuery = await _storeRepository.GetAllAsync(cancellationToken);
        IEnumerable<Store> stores = storesQuery;

        // Apply filtering
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            stores = stores.Where(store =>
                store.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                store.Address.Street.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                store.Address.City.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            stores = stores.Where(store => store.Status.Name.Equals(request.Status, StringComparison.OrdinalIgnoreCase));
        }

        if (request.LocationId.HasValue)
        {
            stores = stores.Where(store => store.LocationId == request.LocationId.Value);
        }

        if (request.IsOperational.HasValue)
        {
            stores = stores.Where(store => store.IsOperational == request.IsOperational.Value);
        }

        // Apply sorting
        stores = request.SortBy.ToUpperInvariant() switch
        {
            "NAME" => request.SortDescending 
                ? stores.OrderByDescending(store => store.Name)
                : stores.OrderBy(store => store.Name),
            "STATUS" => request.SortDescending 
                ? stores.OrderByDescending(store => store.Status.Name)
                : stores.OrderBy(store => store.Status.Name),
            "LOCATIONID" => request.SortDescending 
                ? stores.OrderByDescending(store => store.LocationId)
                : stores.OrderBy(store => store.LocationId),
            "CREATEDAT" or _ => request.SortDescending 
                ? stores.OrderByDescending(store => store.CreatedAt)
                : stores.OrderBy(store => store.CreatedAt)
        };

        // Apply pagination
        var storesList = stores.ToList();
        var totalCount = storesList.Count;
        var pagedItems = storesList
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = pagedItems.Select(MapToDto).ToList();

        return new PagedResult<StoreDto>(dtos, totalCount, request.PageNumber, request.PageSize);
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