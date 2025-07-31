using BuildingBlocks.Application.CQRS.Commands;
using SalesService.Application.DTOs;

namespace SalesService.Application.Commands;

public class ApplyDiscountCommand : CommandBase<AppliedDiscountDto>
{
    public Guid SaleId { get; init; }
    public Guid? SaleDetailId { get; init; }
    public Guid CampaignId { get; init; }
    public Guid RuleId { get; init; }
    public decimal DiscountAmount { get; init; }

    public ApplyDiscountCommand(Guid saleId, Guid campaignId, Guid ruleId, decimal discountAmount, Guid? saleDetailId = null)
    {
        SaleId = saleId;
        SaleDetailId = saleDetailId;
        CampaignId = campaignId;
        RuleId = ruleId;
        DiscountAmount = discountAmount;
    }
}