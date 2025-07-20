using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Specifications;

public class OrSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> Criteria
    {
        get
        {
            var param = Expression.Parameter(typeof(T));
            var leftExpr = Expression.Invoke(_left.Criteria, param);
            var rightExpr = Expression.Invoke(_right.Criteria, param);
            var orExpr = Expression.OrElse(leftExpr, rightExpr);
            return Expression.Lambda<Func<T, bool>>(orExpr, param);
        }
    }
}