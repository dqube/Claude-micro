using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Specifications;

public class NotSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _specification;

    public NotSpecification(ISpecification<T> specification)
    {
        _specification = specification;
    }

    public override Expression<Func<T, bool>> Criteria
    {
        get
        {
            var param = Expression.Parameter(typeof(T));
            var expr = Expression.Invoke(_specification.Criteria, param);
            var notExpr = Expression.Not(expr);
            return Expression.Lambda<Func<T, bool>>(notExpr, param);
        }
    }
}