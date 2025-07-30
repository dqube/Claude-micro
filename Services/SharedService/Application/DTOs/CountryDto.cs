namespace SharedService.Application.DTOs;

public class CountryDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public CurrencyDto? Currency { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 