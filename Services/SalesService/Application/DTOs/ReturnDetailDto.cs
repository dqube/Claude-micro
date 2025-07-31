namespace SalesService.Application.DTOs;

public record ReturnDetailDto
{
    public Guid Id { get; init; }
    public Guid ReturnId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public string Reason { get; init; } = string.Empty;
    public bool Restock { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public string? ModifiedBy { get; init; }
}