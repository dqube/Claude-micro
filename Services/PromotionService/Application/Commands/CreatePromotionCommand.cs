using BuildingBlocks.Application.CQRS.Commands;
using PromotionService.Application.DTOs;

namespace PromotionService.Application.Commands;

public class CreatePromotionCommand : CommandBase<PromotionDto>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool IsCombinable { get; init; } = false;
    public int? MaxRedemptions { get; init; }

    public CreatePromotionCommand(string name, string description, DateTime startDate, DateTime endDate, bool isCombinable = false, int? maxRedemptions = null)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        IsCombinable = isCombinable;
        MaxRedemptions = maxRedemptions;
    }
} 