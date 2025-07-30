using BuildingBlocks.Application.CQRS.Queries;
using EmployeeService.Application.DTOs;
using EmployeeService.Domain.Repositories;

namespace EmployeeService.Application.Queries;

public sealed class GetEmployeesByStoreQueryHandler : IQueryHandler<GetEmployeesByStoreQuery, IEnumerable<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeesByStoreQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<EmployeeDto>> HandleAsync(GetEmployeesByStoreQuery query, CancellationToken cancellationToken = default)
    {
        var employees = await _employeeRepository.GetByStoreIdAsync(query.StoreId, cancellationToken);

        return employees.Select(employee => new EmployeeDto(
            employee.Id.Value,
            employee.UserId,
            employee.StoreId,
            employee.EmployeeNumber.Value,
            employee.HireDate,
            employee.TerminationDate,
            employee.Position.Value,
            employee.AuthLevel,
            employee.IsActive,
            employee.ContactNumbers.Select(cn => new EmployeeContactNumberDto(
                cn.Id.Value,
                cn.EmployeeId.Value,
                cn.ContactNumberTypeId,
                cn.PhoneNumber,
                cn.IsPrimary,
                cn.Verified)).ToList(),
            employee.Addresses.Select(a => new EmployeeAddressDto(
                a.Id.Value,
                a.EmployeeId.Value,
                a.AddressTypeId,
                a.Line1,
                a.Line2,
                a.City,
                a.State,
                a.PostalCode,
                a.CountryCode,
                a.IsPrimary)).ToList()));
    }
}