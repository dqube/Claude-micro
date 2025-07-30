namespace EmployeeService.Application.DTOs;

public sealed record EmployeeAddressDto(
    Guid AddressId,
    Guid EmployeeId,
    int AddressTypeId,
    string Line1,
    string? Line2,
    string City,
    string? State,
    string PostalCode,
    string CountryCode,
    bool IsPrimary);