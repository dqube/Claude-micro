using BuildingBlocks.Domain.Specifications;
using SalesService.Domain.Entities;
using System.Linq.Expressions;

namespace SalesService.Domain.Specifications;

public class PaginatedSalesSpecification : Specification<Sale>
{
    private readonly int _pageNumber;
    private readonly int _pageSize;
    private readonly DateTime? _fromDate;
    private readonly DateTime? _toDate;

    public PaginatedSalesSpecification(int pageNumber, int pageSize, DateTime? fromDate = null, DateTime? toDate = null)
    {
        _pageNumber = pageNumber;
        _pageSize = pageSize;
        _fromDate = fromDate;
        _toDate = toDate;

        ApplyOrderByDescending(s => s.TransactionTime);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        AddInclude(s => s.SaleDetails);
        AddInclude(s => s.AppliedDiscounts);
    }

    public override Expression<Func<Sale, bool>> Criteria
    {
        get
        {
            Expression<Func<Sale, bool>> criteria = s => !s.IsDeleted;

            if (_fromDate.HasValue)
            {
                var fromDate = _fromDate.Value;
                criteria = CombineWithAnd(criteria, s => s.TransactionTime >= fromDate);
            }

            if (_toDate.HasValue)
            {
                var toDate = _toDate.Value;
                criteria = CombineWithAnd(criteria, s => s.TransactionTime <= toDate);
            }

            return criteria;
        }
    }

    private static Expression<Func<Sale, bool>> CombineWithAnd(
        Expression<Func<Sale, bool>> left,
        Expression<Func<Sale, bool>> right)
    {
        var parameter = Expression.Parameter(typeof(Sale), "s");
        var leftBody = new ParameterReplacer(parameter).Visit(left.Body);
        var rightBody = new ParameterReplacer(parameter).Visit(right.Body);
        var combined = Expression.AndAlso(leftBody!, rightBody!);
        return Expression.Lambda<Func<Sale, bool>>(combined, parameter);
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        public ParameterReplacer(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _parameter;
        }
    }
}