using BuildingBlocks.Domain.DomainEvents;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Events;

public class PromotionCreatedEvent : DomainEventBase
{
    public PromotionId PromotionId { get; }
    public string Name { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public PromotionCreatedEvent(PromotionId promotionId, string name, DateTime startDate, DateTime endDate)
    {
        PromotionId = promotionId;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }
} 