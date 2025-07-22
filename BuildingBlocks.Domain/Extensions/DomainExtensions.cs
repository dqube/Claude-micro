using BuildingBlocks.Domain.BusinessRules;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.StronglyTypedIds;

namespace BuildingBlocks.Domain.Extensions;

public static class DomainExtensions
{
    public static void CheckRule(this IBusinessRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }

    public static void CheckRules(this IEnumerable<IBusinessRule> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);
        
        foreach (var rule in rules)
        {
            rule.CheckRule();
        }
    }

    public static bool HasEvents<TId>(this AggregateRoot<TId> aggregateRoot)
        where TId : class, IStronglyTypedId
    {
        ArgumentNullException.ThrowIfNull(aggregateRoot);
        
        return aggregateRoot.DomainEvents.Any();
    }
}