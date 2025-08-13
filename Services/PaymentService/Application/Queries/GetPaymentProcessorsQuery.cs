using BuildingBlocks.Application.CQRS.Queries;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Queries;

public class GetPaymentProcessorsQuery : QueryBase<IEnumerable<PaymentProcessorDto>>
{
    public bool? IsActive { get; init; }

    public GetPaymentProcessorsQuery(bool? isActive = null)
    {
        IsActive = isActive;
    }
} 