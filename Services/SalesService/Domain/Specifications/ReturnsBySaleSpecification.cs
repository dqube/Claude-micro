using BuildingBlocks.Domain.Specifications;
using SalesService.Domain.Entities;
using SalesService.Domain.ValueObjects;
using System.Linq.Expressions;

namespace SalesService.Domain.Specifications;

public class ReturnsBySaleSpecification : Specification<Return>
{
    private readonly SaleId _saleId;

    public ReturnsBySaleSpecification(SaleId saleId)
    {
        _saleId = saleId;

        ApplyOrderByDescending(r => r.ReturnDate);
        AddInclude(r => r.ReturnDetails);
    }

    public override Expression<Func<Return, bool>> Criteria =>
        r => r.SaleId == _saleId && !r.IsDeleted;
}