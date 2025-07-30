using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Logging;

// Example usage of the redaction system
public class RedactionExamples
{
    private readonly ILogger<RedactionExamples> _logger;
    private readonly IDataRedactionService _redactionService;

    public RedactionExamples(ILogger<RedactionExamples> logger, IDataRedactionService redactionService)
    {
        _logger = logger;
        _redactionService = redactionService;
    }

    // Example 1: Basic logging with sensitive data - will be automatically redacted
    public void LogUserAuthentication(string username, string password, string email)
    {
        // ❌ This would normally expose sensitive data
        // But with redaction enabled, sensitive fields are automatically redacted
        _logger.LogInformation("User authentication attempt: {Username} with password {Password} and email {Email}", 
            username, password, email);
        
        // Output: User authentication attempt: john_doe with password [REDACTED] and email [REDACTED]
    }

    // Example 2: Logging structured objects with sensitive data
    public void LogUserRegistration(UserRegistrationRequest request)
    {
        // The entire object is serialized and sensitive fields are redacted
        _logger.LogInformation("User registration request received: {@Request}", request);
        
        // Sensitive fields like Password, Email, CreditCard will be redacted in the output
    }

    // Example 3: Manual redaction for custom scenarios
    public void ManualRedactionExample(string sensitiveData)
    {
        // You can also manually redact data before logging
        var redactedData = _redactionService.RedactMessage(sensitiveData);
        _logger.LogInformation("Processing data: {Data}", redactedData);
    }

    // Example 4: Redacting JSON responses from external APIs
    public async Task LogApiResponse(string apiResponse)
    {
        var redactedResponse = _redactionService.RedactJson(apiResponse);
        _logger.LogInformation("API Response received: {Response}", redactedResponse);
    }

    // Example 5: Custom redaction for specific business rules
    public void LogCustomerData(CustomerData customer)
    {
        // Create custom redaction rules for business-specific scenarios
        var customerJson = JsonSerializer.Serialize(customer);
        var redactedJson = _redactionService.RedactJson(customerJson);
        
        _logger.LogInformation("Customer data processed: {CustomerData}", redactedJson);
    }

    // Example 6: Scope-based logging with redaction
    public void ProcessPayment(PaymentRequest payment)
    {
        using (_logger.BeginScope("Processing payment for {CustomerId}", payment.CustomerId))
        {
            // All logs within this scope will have redaction applied
            _logger.LogInformation("Payment details: {@Payment}", payment);
            _logger.LogInformation("Credit card ending in: {LastFour}", payment.CreditCard?.Substring(payment.CreditCard.Length - 4));
        }
    }
}

// Example data models
public class UserRegistrationRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // Will be redacted
    public string Email { get; set; } = string.Empty;    // Will be redacted
    public string Phone { get; set; } = string.Empty;    // Will be redacted
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}

public class PaymentRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CreditCard { get; set; } = string.Empty; // Will be redacted
    public string CVV { get; set; } = string.Empty;        // Will be redacted
    public DateTime ExpiryDate { get; set; }
}

public class CustomerData
{
    public string CustomerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;     // Will be redacted
    public string Phone { get; set; } = string.Empty;     // Will be redacted
    public string Address { get; set; } = string.Empty;   // Will be redacted
    public string SSN { get; set; } = string.Empty;       // Will be redacted
    public decimal AccountBalance { get; set; }
}