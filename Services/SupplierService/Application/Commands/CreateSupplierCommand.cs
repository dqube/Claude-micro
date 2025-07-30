using BuildingBlocks.Application.CQRS.Commands;
using SupplierService.Application.DTOs;

namespace SupplierService.Application.Commands;

public class CreateSupplierCommand : CommandBase<SupplierDto>
{
    public string Name { get; init; } = string.Empty;
    public string? TaxIdentificationNumber { get; init; }
    public string? Website { get; init; }
    public string? Notes { get; init; }

    public CreateSupplierCommand(string name, string? taxIdentificationNumber = null, string? website = null, string? notes = null)
    {
        Name = name;
        TaxIdentificationNumber = taxIdentificationNumber;
        Website = website;
        Notes = notes;
    }
} 