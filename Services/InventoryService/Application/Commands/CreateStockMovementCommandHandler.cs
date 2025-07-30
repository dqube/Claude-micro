using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using InventoryService.Application.DTOs;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Repositories;
using InventoryService.Domain.ValueObjects;

namespace InventoryService.Application.Commands;

public class CreateStockMovementCommandHandler : ICommandHandler<CreateStockMovementCommand, StockMovementDto>
{
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStockMovementCommandHandler(
        IStockMovementRepository stockMovementRepository,
        IUnitOfWork unitOfWork)
    {
        _stockMovementRepository = stockMovementRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StockMovementDto> HandleAsync(CreateStockMovementCommand request, CancellationToken cancellationToken = default)
    {
        var productId = ProductId.From(request.ProductId);
        var storeId = StoreId.From(request.StoreId);
        var employeeId = EmployeeId.From(request.EmployeeId);
        var movementType = MovementTypeValue.From(request.MovementType);
        var createdBy = request.CreatedBy.HasValue ? EmployeeId.From(request.CreatedBy.Value) : null;

        var stockMovement = new StockMovement(
            StockMovementId.New(),
            productId,
            storeId,
            request.QuantityChange,
            movementType,
            employeeId,
            DateTime.UtcNow,
            request.ReferenceId,
            createdBy);

        await _stockMovementRepository.AddAsync(stockMovement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new StockMovementDto(
            stockMovement.Id.Value,
            stockMovement.ProductId.Value,
            stockMovement.StoreId.Value,
            stockMovement.QuantityChange,
            stockMovement.MovementType.ToString(),
            stockMovement.MovementDate,
            stockMovement.EmployeeId.Value,
            stockMovement.ReferenceId,
            stockMovement.CreatedAt,
            stockMovement.CreatedBy?.Value);
    }
}