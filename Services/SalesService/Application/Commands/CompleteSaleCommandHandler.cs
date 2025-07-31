using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using SalesService.Application.DTOs;
using SalesService.Domain.Exceptions;
using SalesService.Domain.Repositories;
using SalesService.Domain.ValueObjects;

namespace SalesService.Application.Commands;

public class CompleteSaleCommandHandler : ICommandHandler<CompleteSaleCommand, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteSaleCommandHandler(ISaleRepository saleRepository, IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(saleRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SaleDto> HandleAsync(CompleteSaleCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var saleId = SaleId.From(request.SaleId);
        var sale = await _saleRepository.GetWithDetailsAsync(saleId, cancellationToken);
        if (sale == null)
        {
            throw new SaleNotFoundException(saleId);
        }

        sale.CompleteSale(request.CompletedBy);
        
        _saleRepository.Update(sale);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SaleDto
        {
            Id = sale.Id.Value,
            StoreId = sale.StoreId,
            EmployeeId = sale.EmployeeId,
            CustomerId = sale.CustomerId,
            RegisterId = sale.RegisterId,
            TransactionTime = sale.TransactionTime,
            SubTotal = sale.SubTotal,
            DiscountTotal = sale.DiscountTotal,
            TaxAmount = sale.TaxAmount,
            TotalAmount = sale.TotalAmount,
            ReceiptNumber = sale.ReceiptNumber.Value,
            CreatedAt = sale.CreatedAt,
            CreatedBy = sale.CreatedBy,
            SaleDetails = sale.SaleDetails.Select(sd => new SaleDetailDto
            {
                Id = sd.Id.Value,
                SaleId = sd.SaleId.Value,
                ProductId = sd.ProductId,
                Quantity = sd.Quantity,
                UnitPrice = sd.UnitPrice,
                AppliedDiscount = sd.AppliedDiscount,
                TaxApplied = sd.TaxApplied,
                LineTotal = sd.LineTotal,
                CreatedAt = sd.CreatedAt,
                CreatedBy = sd.CreatedBy
            }).ToList(),
            AppliedDiscounts = sale.AppliedDiscounts.Select(ad => new AppliedDiscountDto
            {
                Id = ad.Id.Value,
                SaleDetailId = ad.SaleDetailId?.Value,
                SaleId = ad.SaleId?.Value,
                CampaignId = ad.CampaignId,
                RuleId = ad.RuleId,
                DiscountAmount = ad.DiscountAmount,
                CreatedAt = ad.CreatedAt,
                CreatedBy = ad.CreatedBy
            }).ToList()
        };
    }
}