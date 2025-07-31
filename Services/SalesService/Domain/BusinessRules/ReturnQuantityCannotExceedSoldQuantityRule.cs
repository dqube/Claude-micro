using BuildingBlocks.Domain.BusinessRules;

namespace SalesService.Domain.BusinessRules;

public class ReturnQuantityCannotExceedSoldQuantityRule : BusinessRuleBase
{
    private readonly int _returnQuantity;
    private readonly int _soldQuantity;
    private readonly Guid _productId;

    public ReturnQuantityCannotExceedSoldQuantityRule(int returnQuantity, int soldQuantity, Guid productId)
    {
        _returnQuantity = returnQuantity;
        _soldQuantity = soldQuantity;
        _productId = productId;
    }

    public override bool IsBroken() => _returnQuantity > _soldQuantity;

    public override string Message => $"Return quantity ({_returnQuantity}) cannot exceed sold quantity ({_soldQuantity}) for product {_productId}";
}