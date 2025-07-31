using BuildingBlocks.Application.CQRS.Queries;
using SalesService.Application.DTOs;
using SalesService.Domain.Repositories;
using SalesService.Domain.Specifications;

namespace SalesService.Application.Queries;

public class GetSalesByStoreIdQueryHandler : IQueryHandler<GetSalesByStoreIdQuery, IEnumerable<SaleDto>>
{
    private readonly ISaleRepository _saleRepository;

    public GetSalesByStoreIdQueryHandler(ISaleRepository saleRepository)
    {
        ArgumentNullException.ThrowIfNull(saleRepository);
        _saleRepository = saleRepository;
    }

    public async Task<IEnumerable<SaleDto>> HandleAsync(GetSalesByStoreIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SalesByStoreSpecification(request.StoreId, request.FromDate, request.ToDate);
        var sales = await _saleRepository.FindAsync(spec, cancellationToken);
        
        return sales.Select(sale => new SaleDto
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
            CreatedBy = sale.CreatedBy
        });
    }
}