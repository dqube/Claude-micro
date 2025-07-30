namespace SupplierService.Application.DTOs;

public class SupplierContactNumberDto
{
    public Guid Id { get; set; }
    public Guid ContactId { get; set; }
    public int ContactNumberTypeId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
} 