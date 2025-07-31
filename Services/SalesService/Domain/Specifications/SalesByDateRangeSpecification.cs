using BuildingBlocks.Domain.Specifications;
using SalesService.Domain.Entities;
using System.Linq.Expressions;

namespace SalesService.Domain.Specifications;

public class SalesByDateRangeSpecification : Specification<Sale>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public SalesByDateRangeSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;

        ApplyOrderByDescending(s => s.TransactionTime);
        AddInclude(s => s.SaleDetails);
        AddInclude(s => s.AppliedDiscounts);
    }

    public override Expression<Func<Sale, bool>> Criteria =>
        s => s.TransactionTime >= _startDate && 
             s.TransactionTime <= _endDate && 
             !s.IsDeleted;
}