namespace SalesService.Application.DTOs;

public record ReturnDto
{
    public Guid Id { get; init; }
    public Guid SaleId { get; init; }
    public DateTime ReturnDate { get; init; }
    public Guid EmployeeId { get; init; }
    public Guid? CustomerId { get; init; }
    public decimal TotalRefund { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public string? ModifiedBy { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime? DeletedAt { get; init; }
    public string? DeletedBy { get; init; }
    public List<ReturnDetailDto> ReturnDetails { get; init; } = new();
}