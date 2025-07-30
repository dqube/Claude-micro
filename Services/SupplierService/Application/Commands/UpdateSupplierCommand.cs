using BuildingBlocks.Application.CQRS.Commands;
using SupplierService.Application.DTOs;

namespace SupplierService.Application.Commands;

public class UpdateSupplierCommand : CommandBase<SupplierDto>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? TaxIdentificationNumber { get; init; }
    public string? Website { get; init; }
    public string? Notes { get; init; }

    public UpdateSupplierCommand(Guid id, string name, string? taxIdentificationNumber = null, string? website = null, string? notes = null)
    {
        Id = id;
        Name = name;
        TaxIdentificationNumber = taxIdentificationNumber;
        Website = website;
        Notes = notes;
    }
} 