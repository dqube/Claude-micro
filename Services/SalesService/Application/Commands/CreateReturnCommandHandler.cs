using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using SalesService.Application.DTOs;
using SalesService.Domain.Entities;
using SalesService.Domain.Exceptions;
using SalesService.Domain.Repositories;
using SalesService.Domain.ValueObjects;

namespace SalesService.Application.Commands;

public class CreateReturnCommandHandler : ICommandHandler<CreateReturnCommand, ReturnDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IReturnRepository _returnRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReturnCommandHandler(
        ISaleRepository saleRepository, 
        IReturnRepository returnRepository, 
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(saleRepository);
        ArgumentNullException.ThrowIfNull(returnRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        _saleRepository = saleRepository;
        _returnRepository = returnRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReturnDto> HandleAsync(CreateReturnCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var saleId = SaleId.From(request.SaleId);
        var sale = await _saleRepository.GetByIdAsync(saleId, cancellationToken);
        if (sale == null)
        {
            throw new SaleNotFoundException(saleId);
        }

        var returnEntity = new Return(
            ReturnId.New(),
            saleId,
            request.EmployeeId,
            request.CustomerId);

        await _returnRepository.AddAsync(returnEntity, cancellationToken);
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
            CreatedBy = returnEntity.CreatedBy
        };
    }
}