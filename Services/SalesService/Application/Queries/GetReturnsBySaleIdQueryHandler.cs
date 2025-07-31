using BuildingBlocks.Application.CQRS.Queries;
using SalesService.Application.DTOs;
using SalesService.Domain.Repositories;
using SalesService.Domain.ValueObjects;
using SalesService.Domain.Specifications;

namespace SalesService.Application.Queries;

public class GetReturnsBySaleIdQueryHandler : IQueryHandler<GetReturnsBySaleIdQuery, IEnumerable<ReturnDto>>
{
    private readonly IReturnRepository _returnRepository;

    public GetReturnsBySaleIdQueryHandler(IReturnRepository returnRepository)
    {
        ArgumentNullException.ThrowIfNull(returnRepository);
        _returnRepository = returnRepository;
    }

    public async Task<IEnumerable<ReturnDto>> HandleAsync(GetReturnsBySaleIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var saleId = SaleId.From(request.SaleId);
        var spec = new ReturnsBySaleSpecification(saleId);
        var returns = await _returnRepository.FindAsync(spec, cancellationToken);
        
        return returns.Select(returnEntity => new ReturnDto
        {
            Id = returnEntity.Id.Value,
            SaleId = returnEntity.SaleId.Value,
            ReturnDate = returnEntity.ReturnDate,
            EmployeeId = returnEntity.EmployeeId,
            CustomerId = returnEntity.CustomerId,
            TotalRefund = returnEntity.TotalRefund,
            CreatedAt = returnEntity.CreatedAt,
            CreatedBy = returnEntity.CreatedBy
        });
    }
}