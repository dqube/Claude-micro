namespace BuildingBlocks.Domain.BusinessRules;

public abstract class BusinessRuleBase : IBusinessRule
{
    public abstract bool IsBroken();
    public abstract string Message { get; }

    protected static void CheckRule(IBusinessRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        
        if (rule.IsBroken())
        {
            throw new Exceptions.BusinessRuleValidationException(rule);
        }
    }
}