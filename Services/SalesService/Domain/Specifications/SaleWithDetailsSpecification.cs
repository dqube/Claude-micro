using BuildingBlocks.Domain.Specifications;
using SalesService.Domain.Entities;
using SalesService.Domain.ValueObjects;
using System.Linq.Expressions;

namespace SalesService.Domain.Specifications;

public class SaleWithDetailsSpecification : Specification<Sale>
{
    private readonly SaleId _saleId;

    public SaleWithDetailsSpecification(SaleId saleId)
    {
        _saleId = saleId;

        AddInclude(s => s.SaleDetails);
        AddInclude(s => s.AppliedDiscounts);
    }

    public override Expression<Func<Sale, bool>> Criteria =>
        s => s.Id == _saleId && !s.IsDeleted;
}