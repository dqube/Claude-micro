using BuildingBlocks.Domain.Common;

namespace ReportingService.Application.DTOs;

public record InventorySnapshotDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public int StoreId { get; init; }
    public int Quantity { get; init; }
    public DateOnly SnapshotDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid? CreatedBy { get; init; }
} 