namespace SupplierService.Application.DTOs;

public class SupplierContactDto
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Position { get; set; }
    public bool IsPrimary { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public string FullName { get; set; } = string.Empty;
    public List<SupplierContactNumberDto> ContactNumbers { get; set; } = new();
} 