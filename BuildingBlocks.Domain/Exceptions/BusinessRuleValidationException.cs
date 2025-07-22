using BuildingBlocks.Domain.BusinessRules;

namespace BuildingBlocks.Domain.Exceptions;

public class BusinessRuleValidationException : DomainException
{
    public IBusinessRule? BrokenRule { get; }

    public BusinessRuleValidationException()
        : base("A business rule was violated.")
    {
    }

    public BusinessRuleValidationException(string message)
        : base(message)
    {
    }

    public BusinessRuleValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public BusinessRuleValidationException(IBusinessRule brokenRule) 
        : base(brokenRule?.Message ?? "A business rule was violated.")
    {
        ArgumentNullException.ThrowIfNull(brokenRule);
        BrokenRule = brokenRule;
    }

    public override string ToString()
    {
        return BrokenRule != null 
            ? $"{BrokenRule.GetType().Name}: {BrokenRule.Message}"
            : base.ToString();
    }
}