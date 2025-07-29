using BuildingBlocks.Application.CQRS.Queries;
using CustomerService.Application.DTOs;

namespace CustomerService.Application.Queries;

public class GetCustomerByIdQuery : QueryBase<CustomerDto?>
{
    public Guid CustomerId { get; }

    public GetCustomerByIdQuery(Guid customerId)
    {
        CustomerId = customerId;
    }
} 