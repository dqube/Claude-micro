using BuildingBlocks.Domain.Specifications;
using SalesService.Domain.Entities;
using System.Linq.Expressions;

namespace SalesService.Domain.Specifications;

public class SalesByCustomerSpecification : Specification<Sale>
{
    private readonly Guid _customerId;

    public SalesByCustomerSpecification(Guid customerId)
    {
        _customerId = customerId;

        ApplyOrderByDescending(s => s.TransactionTime);
        AddInclude(s => s.SaleDetails);
        AddInclude(s => s.AppliedDiscounts);
    }

    public override Expression<Func<Sale, bool>> Criteria =>
        s => s.CustomerId == _customerId && !s.IsDeleted;
}