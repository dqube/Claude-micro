using BuildingBlocks.Application.CQRS.Queries;
using EmployeeService.Application.DTOs;
using EmployeeService.Domain.Exceptions;
using EmployeeService.Domain.Repositories;
using EmployeeService.Domain.ValueObjects;

namespace EmployeeService.Application.Queries;

public sealed class GetEmployeeByIdQueryHandler : IQueryHandler<GetEmployeeByIdQuery, EmployeeDto>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeDto> HandleAsync(GetEmployeeByIdQuery query, CancellationToken cancellationToken = default)
    {
        var employeeId = EmployeeId.From(query.EmployeeId);
        var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken)
            ?? throw new EmployeeNotFoundException(employeeId);

        return new EmployeeDto(
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
                a.IsPrimary)).ToList());
    }
}