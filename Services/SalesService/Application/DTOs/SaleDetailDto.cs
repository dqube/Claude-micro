namespace SalesService.Application.DTOs;

public record SaleDetailDto
{
    public Guid Id { get; init; }
    public Guid SaleId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal AppliedDiscount { get; init; }
    public decimal TaxApplied { get; init; }
    public decimal LineTotal { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public string? ModifiedBy { get; init; }
}