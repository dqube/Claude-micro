namespace EmployeeService.Application.DTOs;

public sealed record EmployeeDto(
    Guid EmployeeId,
    Guid UserId,
    int StoreId,
    string EmployeeNumber,
    DateTime HireDate,
    DateTime? TerminationDate,
    string Position,
    int AuthLevel,
    bool IsActive,
    ICollection<EmployeeContactNumberDto> ContactNumbers,
    ICollection<EmployeeAddressDto> Addresses);