namespace SalesService.Application.DTOs;

public record SaleDto
{
    public Guid Id { get; init; }
    public int StoreId { get; init; }
    public Guid EmployeeId { get; init; }
    public Guid? CustomerId { get; init; }
    public int RegisterId { get; init; }
    public DateTime TransactionTime { get; init; }
    public decimal SubTotal { get; init; }
    public decimal DiscountTotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public string ReceiptNumber { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public string? ModifiedBy { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime? DeletedAt { get; init; }
    public string? DeletedBy { get; init; }
    public List<SaleDetailDto> SaleDetails { get; init; } = new();
    public List<AppliedDiscountDto> AppliedDiscounts { get; init; } = new();
}