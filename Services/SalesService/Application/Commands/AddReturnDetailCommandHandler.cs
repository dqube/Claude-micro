using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using SalesService.Application.DTOs;
using SalesService.Domain.Exceptions;
using SalesService.Domain.Repositories;
using SalesService.Domain.ValueObjects;

namespace SalesService.Application.Commands;

public class AddReturnDetailCommandHandler : ICommandHandler<AddReturnDetailCommand, ReturnDetailDto>
{
    private readonly IReturnRepository _returnRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddReturnDetailCommandHandler(IReturnRepository returnRepository, IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(returnRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        _returnRepository = returnRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReturnDetailDto> HandleAsync(AddReturnDetailCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var returnId = ReturnId.From(request.ReturnId);
        var returnEntity = await _returnRepository.GetByIdAsync(returnId, cancellationToken);
        if (returnEntity == null)
        {
            throw new ReturnNotFoundException(returnId);
        }

        var reason = ReturnReason.FromName(request.Reason);
        returnEntity.AddReturnDetail(request.ProductId, request.Quantity, reason, request.Restock);
        
        _returnRepository.Update(returnEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var addedDetail = returnEntity.ReturnDetails.Last();
        return new ReturnDetailDto
        {
            Id = addedDetail.Id.Value,
            ReturnId = addedDetail.ReturnId.Value,
            ProductId = addedDetail.ProductId,
            Quantity = addedDetail.Quantity,
            Reason = addedDetail.Reason.Name,
            Restock = addedDetail.Restock,
            CreatedAt = addedDetail.CreatedAt,
            CreatedBy = addedDetail.CreatedBy
        };
    }
}