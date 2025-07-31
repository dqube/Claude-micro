using BuildingBlocks.Application.CQRS.Commands;
using ReportingService.Application.DTOs;

namespace ReportingService.Application.Commands;

public class CreateSalesSnapshotCommand : CommandBase<SalesSnapshotDto>
{
    public Guid SaleId { get; init; }
    public int StoreId { get; init; }
    public DateOnly SaleDate { get; init; }
    public decimal TotalAmount { get; init; }
    public Guid? CustomerId { get; init; }

    public CreateSalesSnapshotCommand(
        Guid saleId,
        int storeId,
        DateOnly saleDate,
        decimal totalAmount,
        Guid? customerId = null)
    {
        SaleId = saleId;
        StoreId = storeId;
        SaleDate = saleDate;
        TotalAmount = totalAmount;
        CustomerId = customerId;
    }
} 