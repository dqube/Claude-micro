using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using SalesService.Application.DTOs;
using SalesService.Domain.Exceptions;
using SalesService.Domain.Repositories;
using SalesService.Domain.ValueObjects;

namespace SalesService.Application.Commands;

public class ProcessReturnCommandHandler : ICommandHandler<ProcessReturnCommand, ReturnDto>
{
    private readonly IReturnRepository _returnRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessReturnCommandHandler(IReturnRepository returnRepository, IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(returnRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        _returnRepository = returnRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReturnDto> HandleAsync(ProcessReturnCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var returnId = ReturnId.From(request.ReturnId);
        var returnEntity = await _returnRepository.GetWithDetailsAsync(returnId, cancellationToken);
        if (returnEntity == null)
        {
            throw new ReturnNotFoundException(returnId);
        }

        returnEntity.ProcessReturn(request.TotalRefund, request.ProcessedBy);
        
        _returnRepository.Update(returnEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ReturnDto
        {
            Id = returnEntity.Id.Value,
            SaleId = returnEntity.SaleId.Value,
            ReturnDate = returnEntity.ReturnDate,
            EmployeeId = returnEntity.EmployeeId,
            CustomerId = returnEntity.CustomerId,
            TotalRefund = returnEntity.TotalRefund,
            CreatedAt = returnEntity.CreatedAt,
            CreatedBy = returnEntity.CreatedBy,
            ReturnDetails = returnEntity.ReturnDetails.Select(rd => new ReturnDetailDto
            {
                Id = rd.Id.Value,
                ReturnId = rd.ReturnId.Value,
                ProductId = rd.ProductId,
                Quantity = rd.Quantity,
                Reason = rd.Reason.Name,
                Restock = rd.Restock,
                CreatedAt = rd.CreatedAt,
                CreatedBy = rd.CreatedBy
            }).ToList()
        };
    }
}