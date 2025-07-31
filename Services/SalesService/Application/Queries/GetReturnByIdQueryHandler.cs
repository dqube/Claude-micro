using BuildingBlocks.Application.CQRS.Queries;
using SalesService.Application.DTOs;
using SalesService.Domain.Repositories;
using SalesService.Domain.ValueObjects;
using SalesService.Domain.Specifications;

namespace SalesService.Application.Queries;

public class GetReturnByIdQueryHandler : IQueryHandler<GetReturnByIdQuery, ReturnDto?>
{
    private readonly IReturnRepository _returnRepository;

    public GetReturnByIdQueryHandler(IReturnRepository returnRepository)
    {
        ArgumentNullException.ThrowIfNull(returnRepository);
        _returnRepository = returnRepository;
    }

    public async Task<ReturnDto?> HandleAsync(GetReturnByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var returnId = ReturnId.From(request.ReturnId);
        var spec = new ReturnWithDetailsSpecification(returnId);
        var returnEntity = await _returnRepository.FindFirstAsync(spec, cancellationToken);
        
        if (returnEntity == null)
            return null;

        return new ReturnDto
        {
            Id = returnEntity.Id.Value,
            SaleId = returnEntity.SaleId.Value,
            ReturnDate = returnEntity.ReturnDate,
            EmployeeId = returnEntity.EmployeeId,
            CustomerId = returnEntity.CustomerId,
            TotalRefund = returnEntity.TotalRefund,
            CreatedAt = returnEntity.CreatedAt,
            CreatedBy = returnEntity.CreatedBy,
            ReturnDetails = returnEntity.ReturnDetails.Select(rd => new ReturnDetailDto
            {
                Id = rd.Id.Value,
                ReturnId = rd.ReturnId.Value,
                ProductId = rd.ProductId,
                Quantity = rd.Quantity,
                Reason = rd.Reason.Name,
                Restock = rd.Restock,
                CreatedAt = rd.CreatedAt,
                CreatedBy = rd.CreatedBy
            }).ToList()
        };
    }
}