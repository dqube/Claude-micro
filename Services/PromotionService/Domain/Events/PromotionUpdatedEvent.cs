using BuildingBlocks.Domain.DomainEvents;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Events;

public class PromotionUpdatedEvent : DomainEventBase
{
    public PromotionId PromotionId { get; }
    public string Name { get; }

    public PromotionUpdatedEvent(PromotionId promotionId, string name)
    {
        PromotionId = promotionId;
        Name = name;
    }
} 