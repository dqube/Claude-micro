using BuildingBlocks.Application.CQRS.Queries;
using SalesService.Application.DTOs;
using SalesService.Domain.Repositories;
using SalesService.Domain.ValueObjects;
using SalesService.Domain.Specifications;

namespace SalesService.Application.Queries;

public class GetSaleByIdQueryHandler : IQueryHandler<GetSaleByIdQuery, SaleDto?>
{
    private readonly ISaleRepository _saleRepository;

    public GetSaleByIdQueryHandler(ISaleRepository saleRepository)
    {
        ArgumentNullException.ThrowIfNull(saleRepository);
        _saleRepository = saleRepository;
    }

    public async Task<SaleDto?> HandleAsync(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var saleId = SaleId.From(request.SaleId);
        var spec = new SaleWithDetailsSpecification(saleId);
        var sale = await _saleRepository.FindFirstAsync(spec, cancellationToken);
        
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