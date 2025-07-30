using BuildingBlocks.Application.CQRS.Commands;
using SupplierService.Application.DTOs;

namespace SupplierService.Application.Commands;

public class CreateSupplierContactCommand : CommandBase<SupplierContactDto>
{
    public Guid SupplierId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Position { get; init; }
    public bool IsPrimary { get; init; }
    public string? Notes { get; init; }

    public CreateSupplierContactCommand(Guid supplierId, string firstName, string lastName, string? email = null, string? position = null, bool isPrimary = false, string? notes = null)
    {
        SupplierId = supplierId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Position = position;
        IsPrimary = isPrimary;
        Notes = notes;
    }
} 