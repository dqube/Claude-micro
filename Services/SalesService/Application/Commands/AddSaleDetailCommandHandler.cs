using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using SalesService.Application.DTOs;
using SalesService.Domain.Exceptions;
using SalesService.Domain.Repositories;
using SalesService.Domain.ValueObjects;

namespace SalesService.Application.Commands;

public class AddSaleDetailCommandHandler : ICommandHandler<AddSaleDetailCommand, SaleDetailDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddSaleDetailCommandHandler(ISaleRepository saleRepository, IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(saleRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SaleDetailDto> HandleAsync(AddSaleDetailCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var saleId = SaleId.From(request.SaleId);
        var sale = await _saleRepository.GetByIdAsync(saleId, cancellationToken);
        if (sale == null)
        {
            throw new SaleNotFoundException(saleId);
        }

        sale.AddSaleDetail(request.ProductId, request.Quantity, request.UnitPrice, request.TaxApplied);
        
        _saleRepository.Update(sale);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var addedDetail = sale.SaleDetails.Last();
        return new SaleDetailDto
        {
            Id = addedDetail.Id.Value,
            SaleId = addedDetail.SaleId.Value,
            ProductId = addedDetail.ProductId,
            Quantity = addedDetail.Quantity,
            UnitPrice = addedDetail.UnitPrice,
            AppliedDiscount = addedDetail.AppliedDiscount,
            TaxApplied = addedDetail.TaxApplied,
            LineTotal = addedDetail.LineTotal,
            CreatedAt = addedDetail.CreatedAt,
            CreatedBy = addedDetail.CreatedBy
        };
    }
}