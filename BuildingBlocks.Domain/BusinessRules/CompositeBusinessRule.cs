namespace BuildingBlocks.Domain.BusinessRules;

public class CompositeBusinessRule : IBusinessRule
{
    private readonly List<IBusinessRule> _businessRules;
    private readonly string _message;

    public CompositeBusinessRule(string message, params IBusinessRule[] businessRules)
    {
        _message = message;
        _businessRules = businessRules.ToList();
    }

    public CompositeBusinessRule(string message, IEnumerable<IBusinessRule> businessRules)
    {
        _message = message;
        _businessRules = businessRules.ToList();
    }

    public bool IsBroken()
    {
        return _businessRules.Any(rule => rule.IsBroken());
    }

    public string Message => _message;

    public IReadOnlyList<IBusinessRule> BrokenRules => _businessRules.Where(rule => rule.IsBroken()).ToList();
}