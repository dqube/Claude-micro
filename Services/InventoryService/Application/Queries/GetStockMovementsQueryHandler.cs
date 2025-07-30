using BuildingBlocks.Application.CQRS.Queries;
using InventoryService.Application.DTOs;
using InventoryService.Domain.Repositories;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Application.Queries;

public class GetStockMovementsQueryHandler : IQueryHandler<GetStockMovementsQuery, IReadOnlyList<StockMovementDto>>
{
    private readonly IStockMovementRepository _stockMovementRepository;

    public GetStockMovementsQueryHandler(IStockMovementRepository stockMovementRepository)
    {
        _stockMovementRepository = stockMovementRepository;
    }

    public async Task<IReadOnlyList<StockMovementDto>> HandleAsync(GetStockMovementsQuery request, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Domain.Entities.StockMovement> movements;

        if (request.ProductId.HasValue && request.StoreId.HasValue)
        {
            var productId = ProductId.From(request.ProductId.Value);
            var storeId = StoreId.From(request.StoreId.Value);
            movements = await _stockMovementRepository.GetByStoreAndProductAsync(storeId, productId, cancellationToken);
        }
        else if (request.ProductId.HasValue)
        {
            var productId = ProductId.From(request.ProductId.Value);
            movements = await _stockMovementRepository.GetByProductAsync(productId, cancellationToken);
        }
        else if (request.StoreId.HasValue)
        {
            var storeId = StoreId.From(request.StoreId.Value);
            movements = await _stockMovementRepository.GetByStoreAsync(storeId, cancellationToken);
        }
        else if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            movements = await _stockMovementRepository.GetByDateRangeAsync(request.StartDate.Value, request.EndDate.Value, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(request.MovementType))
        {
            var movementType = MovementTypeValue.From(request.MovementType);
            movements = await _stockMovementRepository.GetByMovementTypeAsync(movementType, cancellationToken);
        }
        else
        {
            movements = await _stockMovementRepository.GetAllAsync(cancellationToken);
        }

        return movements.Select(movement => new StockMovementDto(
            movement.Id.Value,
            movement.ProductId.Value,
            movement.StoreId.Value,
            movement.QuantityChange,
            movement.MovementType.ToString(),
            movement.MovementDate,
            movement.EmployeeId.Value,
            movement.ReferenceId,
            movement.CreatedAt,
            movement.CreatedBy?.Value)).ToList();
    }
}