using BuildingBlocks.Domain.Exceptions;
using PromotionService.Domain.ValueObjects;

namespace PromotionService.Domain.Exceptions;

public class DiscountTypeNotFoundException : AggregateNotFoundException
{
    public DiscountTypeNotFoundException() : base("Discount type was not found")
    {
    }

    public DiscountTypeNotFoundException(string message) : base(message)
    {
    }

    public DiscountTypeNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public DiscountTypeNotFoundException(DiscountTypeId discountTypeId) 
        : base($"Discount type with ID '{discountTypeId?.Value}' was not found")
    {
    }

    public DiscountTypeNotFoundException(string name, bool isName) 
        : base($"Discount type with name '{name}' was not found")
    {
    }
} 