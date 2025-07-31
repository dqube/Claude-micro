namespace SalesService.Application.DTOs;

public record AppliedDiscountDto
{
    public Guid Id { get; init; }
    public Guid? SaleDetailId { get; init; }
    public Guid? SaleId { get; init; }
    public Guid CampaignId { get; init; }
    public Guid RuleId { get; init; }
    public decimal DiscountAmount { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public string? ModifiedBy { get; init; }
}