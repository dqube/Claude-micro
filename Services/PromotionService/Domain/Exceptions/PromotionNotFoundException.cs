using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Exceptions;

public class PromotionNotFoundException : Exception
{
    public PromotionNotFoundException(PromotionId promotionId)
        : base($"Promotion with ID '{promotionId.Value}' was not found.")
    {
    }

    public PromotionNotFoundException(PromotionId promotionId, Exception innerException)
        : base($"Promotion with ID '{promotionId.Value}' was not found.", innerException)
    {
    }
} 