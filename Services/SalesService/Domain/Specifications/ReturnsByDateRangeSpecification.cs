using BuildingBlocks.Domain.Specifications;
using SalesService.Domain.Entities;
using System.Linq.Expressions;

namespace SalesService.Domain.Specifications;

public class ReturnsByDateRangeSpecification : Specification<Return>
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public ReturnsByDateRangeSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;

        ApplyOrderByDescending(r => r.ReturnDate);
        AddInclude(r => r.ReturnDetails);
    }

    public override Expression<Func<Return, bool>> Criteria =>
        r => r.ReturnDate >= _startDate && 
             r.ReturnDate <= _endDate && 
             !r.IsDeleted;
}