using BuildingBlocks.Domain.Common;

namespace ReportingService.Application.DTOs;

public record SalesSnapshotDto
{
    public Guid Id { get; init; }
    public Guid SaleId { get; init; }
    public int StoreId { get; init; }
    public DateOnly SaleDate { get; init; }
    public decimal TotalAmount { get; init; }
    public Guid? CustomerId { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid? CreatedBy { get; init; }
} 