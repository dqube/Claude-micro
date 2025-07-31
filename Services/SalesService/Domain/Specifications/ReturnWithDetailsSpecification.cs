using BuildingBlocks.Domain.Specifications;
using SalesService.Domain.Entities;
using SalesService.Domain.ValueObjects;
using System.Linq.Expressions;

namespace SalesService.Domain.Specifications;

public class ReturnWithDetailsSpecification : Specification<Return>
{
    private readonly ReturnId _returnId;

    public ReturnWithDetailsSpecification(ReturnId returnId)
    {
        _returnId = returnId;

        AddInclude(r => r.ReturnDetails);
    }

    public override Expression<Func<Return, bool>> Criteria =>
        r => r.Id == _returnId && !r.IsDeleted;
}