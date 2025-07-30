namespace EmployeeService.Application.DTOs;

public sealed record EmployeeContactNumberDto(
    Guid ContactNumberId,
    Guid EmployeeId,
    int ContactNumberTypeId,
    string PhoneNumber,
    bool IsPrimary,
    bool Verified);