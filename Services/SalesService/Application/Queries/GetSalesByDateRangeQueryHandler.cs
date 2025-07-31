using BuildingBlocks.Application.CQRS.Queries;
using SalesService.Application.DTOs;
using SalesService.Domain.Repositories;
using SalesService.Domain.Specifications;

namespace SalesService.Application.Queries;

public class GetSalesByDateRangeQueryHandler : IQueryHandler<GetSalesByDateRangeQuery, IEnumerable<SaleDto>>
{
    private readonly ISaleRepository _saleRepository;

    public GetSalesByDateRangeQueryHandler(ISaleRepository saleRepository)
    {
        ArgumentNullException.ThrowIfNull(saleRepository);
        _saleRepository = saleRepository;
    }

    public async Task<IEnumerable<SaleDto>> HandleAsync(GetSalesByDateRangeQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SalesByDateRangeSpecification(request.StartDate, request.EndDate);
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