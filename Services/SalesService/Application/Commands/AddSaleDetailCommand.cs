using BuildingBlocks.Application.CQRS.Commands;
using SalesService.Application.DTOs;

namespace SalesService.Application.Commands;

public class AddSaleDetailCommand : CommandBase<SaleDetailDto>
{
    public Guid SaleId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TaxApplied { get; init; }

    public AddSaleDetailCommand(Guid saleId, Guid productId, int quantity, decimal unitPrice, decimal taxApplied = 0)
    {
        SaleId = saleId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TaxApplied = taxApplied;
    }
}