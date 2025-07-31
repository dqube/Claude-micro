using BuildingBlocks.Application.CQRS.Queries;
using SalesService.Application.DTOs;
using SalesService.Domain.Repositories;
using SalesService.Domain.ValueObjects;

namespace SalesService.Application.Queries;

public class GetSaleByReceiptNumberQueryHandler : IQueryHandler<GetSaleByReceiptNumberQuery, SaleDto?>
{
    private readonly ISaleRepository _saleRepository;

    public GetSaleByReceiptNumberQueryHandler(ISaleRepository saleRepository)
    {
        ArgumentNullException.ThrowIfNull(saleRepository);
        _saleRepository = saleRepository;
    }

    public async Task<SaleDto?> HandleAsync(GetSaleByReceiptNumberQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var receiptNumber = ReceiptNumber.From(request.ReceiptNumber);
        var sale = await _saleRepository.GetByReceiptNumberAsync(receiptNumber, cancellationToken);
        
        if (sale == null)
            return null;

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