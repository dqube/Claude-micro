using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Common;
using BuildingBlocks.Domain.Repository;
using StoreService.Domain.Entities;
using StoreService.Domain.Exceptions;
using StoreService.Domain.ValueObjects;

namespace StoreService.Application.Commands;

public class UpdateStoreStatusCommandHandler : ICommandHandler<UpdateStoreStatusCommand>
{
    private readonly IRepository<Store, StoreId> _storeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStoreStatusCommandHandler(
        IRepository<Store, StoreId> storeRepository,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(storeRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        _storeRepository = storeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(UpdateStoreStatusCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var store = await _storeRepository.GetByIdAsync(request.StoreId, cancellationToken);
        if (store is null)
            throw new StoreNotFoundException(request.StoreId);

        var newStatus = StoreStatus.FromName(request.Status);
        store.ChangeStatus(newStatus);

        _storeRepository.Update(store);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
} 