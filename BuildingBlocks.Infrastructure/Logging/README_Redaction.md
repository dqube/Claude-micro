# Data Redaction for Logging

This implementation provides comprehensive data redaction capabilities for logging sensitive information in your application. It integrates seamlessly with OpenTelemetry and Microsoft.Extensions.Logging.

## Features

- **Automatic Redaction**: Sensitive fields are automatically detected and redacted
- **Configurable Patterns**: Support for regex patterns and field name matching
- **OpenTelemetry Integration**: Works with distributed tracing and logging
- **Multiple Strategies**: Full masking, partial masking, hashing, and length-only display
- **Custom Rules**: Add your own redaction rules for business-specific data
- **Performance Optimized**: Minimal overhead with caching and efficient algorithms

## Configuration

### appsettings.json Configuration

```json
{
  "OpenTelemetry": {
    "Redaction": {
      "Enabled": true,
      "RedactionText": "[REDACTED]",
      "Mode": "Full",
      "SensitiveFields": [
        "password",
        "secret",
        "token",
        "apikey",
        "authorization",
        "creditcard",
        "ssn",
        "email",
        "phone",
        "address"
      ]
    }
  }
}
```

### Programmatic Configuration

```csharp
services.Configure<RedactionOptions>(options =>
{
    options.Enabled = true;
    options.RedactionText = "[CONFIDENTIAL]";
    options.Mode = RedactionMode.Partial;
    
    // Add custom sensitive fields
    options.SensitiveFields.Add("internal_id");
    options.SensitiveFields.Add("customer_key");
    
    // Add custom regex patterns
    options.RegexPatterns["custom_id"] = new Regex(@"CUST-\d{8}", RegexOptions.Compiled);
});
```

## Usage Examples

### Basic Logging

```csharp
public class UserService
{
    private readonly ILogger<UserService> _logger;

    public UserService(ILogger<UserService> logger)
    {
        _logger = logger;
    }

    public void AuthenticateUser(string username, string password)
    {
        // Password will be automatically redacted
        _logger.LogInformation("User {Username} authentication attempt with password {Password}", 
            username, password);
        // Output: User john_doe authentication attempt with password [REDACTED]
    }

    public void ProcessUserData(UserModel user)
    {
        // Sensitive fields in the object will be redacted
        _logger.LogInformation("Processing user data: {@User}", user);
        // Email, phone, and other sensitive fields will be redacted in the JSON
    }
}
```

### Manual Redaction

```csharp
public class PaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly IDataRedactionService _redactionService;

    public PaymentService(ILogger<PaymentService> logger, IDataRedactionService redactionService)
    {
        _logger = logger;
        _redactionService = redactionService;
    }

    public void ProcessPayment(string paymentData)
    {
        // Manually redact sensitive data before logging
        var redactedData = _redactionService.RedactJson(paymentData);
        _logger.LogInformation("Payment data: {PaymentData}", redactedData);
    }
}
```

### API Response Redaction

```csharp
public class ExternalApiService
{
    private readonly ILogger<ExternalApiService> _logger;
    private readonly IDataRedactionService _redactionService;

    public async Task<string> CallExternalApi()
    {
        var response = await httpClient.GetStringAsync("/user/profile");
        
        // Redact sensitive data from external API responses
        var redactedResponse = _redactionService.RedactJson(response);
        _logger.LogInformation("External API response: {Response}", redactedResponse);
        
        return response;
    }
}
```

## Redaction Strategies

### Full Masking (Default)
```
"password": "secretPassword123" → "password": "[REDACTED]"
```

### Partial Masking
```
"email": "user@example.com" → "email": "u***@example.com"
"creditcard": "1234567890123456" → "creditcard": "****-****-****-3456"
```

### Hashing
```
"sensitive_data": "confidential" → "sensitive_data": "sha256:a1b2c3d4..."
```

### Length Only
```
"password": "secretPassword123" → "password": "[LENGTH:17]"
```

## Built-in Sensitive Field Detection

The system automatically detects and redacts these comprehensive PII field types:

### **Authentication & Security (260+ patterns)**
- **Credentials**: password, pwd, secret, token, key, authorization, bearer, apikey, csrf, session tokens
- **Cryptographic**: privatekey, publickey, signature, hash, salt, nonce, challenge
- **Multi-Factor**: otp, totp, mfa, 2fa
- **Biometric Access**: fingerprint, retina, faceprint, biometric templates

### **Financial Information (40+ patterns)**
- **Payment Cards**: creditcard, debitcard, cvv, cvc, expiry dates (Visa, MasterCard, Amex, Discover)
- **Banking**: accountnumber, routingnumber, iban, swift, bic, sortcode
- **Financial Data**: salary, income, balance, creditlimit, transactions
- **Tax Information**: taxid, ein, vat numbers

### **Government & Legal IDs (30+ patterns)**
- **US IDs**: ssn, social_security, drivers_license, passport, medicare, medicaid
- **International**: nationalid, citizenid, visa, greencard, nino (UK), sin (Canada)
- **Tax IDs**: tin, itin, ein

### **Medical & Health Data (25+ patterns)**
- **Patient Data**: patientid, mrn, medical_record_number, healthinsurance
- **Health Info**: diagnosis, medication, prescription, allergies, bloodtype
- **Treatment**: medicalhistory, chroniccondition, surgery, vaccination

### **Contact & Personal Information (50+ patterns)**
- **Contact**: email, phone, mobile, telephone, fax (multiple formats)
- **Address**: streetaddress, city, state, zipcode, coordinates, geolocation
- **Names**: firstname, lastname, fullname, nickname, maidenname
- **Personal**: birthdate, age, nationality, gender, maritalstatus, religion

### **Digital Identifiers (30+ patterns)**
- **Network**: ip, ipaddress, ipv4, ipv6, macaddress, deviceid
- **System IDs**: userid, customerid, employeeid, uuid, guid, imei, udid
- **Accounts**: username, login, screenname, handle, alias

### **Biometric & Genetic Data (15+ patterns)**
- **Biometrics**: fingerprint, retina, iris, faceprint, voiceprint
- **Genetic**: dna, genetic data, biometric templates

### **Educational & Professional (20+ patterns)**
- **Academic**: studentid, schoolid, gpa, transcript, degree, certification
- **Work**: employer, jobtitle, department, securityclearance, profession

### **Family & Relationships (15+ patterns)**
- **Family**: mothername, fathername, spousename, children, relatives
- **Emergency**: emergencycontact, nextofkin

## Built-in Regex Patterns (35+ patterns)

### **Contact & Identity Patterns**
- **Email addresses**: `user@domain.com`, `user+tag@example.org`
- **Phone numbers**: `(555) 123-4567`, `+1-555-123-4567`, international formats
- **IP addresses**: IPv4 `192.168.1.1`, IPv6 `2001:0db8:85a3::8a2e:0370:7334`
- **MAC addresses**: `00:1B:44:11:3A:B7`

### **Financial Patterns**
- **Credit cards**: `1234-5678-9012-3456` (Visa, MasterCard, Amex, Discover)
- **Bank accounts**: 8-17 digit account numbers
- **Routing numbers**: 9-digit US bank routing numbers
- **IBAN codes**: International bank account numbers

### **Government ID Patterns**
- **Social Security**: `123-45-6789`, `123456789`
- **Tax IDs**: EIN `12-3456789`, ITIN `912-34-5678`
- **Driver's licenses**: State-specific patterns
- **Passport numbers**: US 9-digit, international formats

### **Medical & Insurance**
- **Medical Record Numbers**: `MRN: 1234567`
- **Patient IDs**: `PID: 987654321`
- **Medicare numbers**: `123-45-6789A`
- **Medicaid numbers**: 8-12 digit patterns

### **Security & Tech Patterns**
- **JWT tokens**: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
- **API keys**: 32+ character alphanumeric strings
- **AWS keys**: `AKIA1234567890123456`
- **GitHub tokens**: `ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx`
- **Bearer tokens**: `Bearer abc123def456`
- **UUIDs/GUIDs**: `550e8400-e29b-41d4-a716-446655440000`

### **Geographic & Vehicle**
- **Coordinates**: `40.7128,-74.0060` (latitude, longitude)
- **VIN numbers**: 17-character vehicle identification
- **License plates**: 2-8 character alphanumeric

### **Hash & Crypto Patterns**
- **MD5 hashes**: 32-character hex strings
- **SHA1 hashes**: 40-character hex strings
- **SHA256 hashes**: 64-character hex strings
- **Connection strings**: Database credentials in connection strings
- **URLs with credentials**: `https://user:pass@example.com`

### **Date & Time Sensitive**
- **Date of birth**: `MM/DD/YYYY`, `YYYY-MM-DD` formats
- **Biometric templates**: Long hexadecimal patterns

## OpenTelemetry Integration

The redaction system automatically integrates with OpenTelemetry:

- **Traces**: Activity names, tags, and baggage are redacted
- **Logs**: Log messages, structured data, and scopes are redacted
- **Metrics**: Metric labels and values are redacted when they contain sensitive data

## Performance Considerations

- **Regex Compilation**: Patterns are compiled once for better performance
- **Caching**: Redaction results are cached for repeated values
- **Lazy Evaluation**: Redaction only occurs when logging is enabled
- **Memory Efficient**: Minimal object allocation during redaction

## Security Best Practices

1. **Enable in Production**: Always enable redaction in production environments
2. **Regular Updates**: Keep sensitive field lists updated as your schema evolves
3. **Audit Logs**: Regularly audit logs to ensure no sensitive data is exposed
4. **Custom Rules**: Add business-specific sensitive data patterns
5. **Test Coverage**: Write tests to verify redaction works for your specific data models

## Troubleshooting

### Redaction Not Working
1. Verify `Redaction.Enabled` is set to `true` in configuration
2. Check that sensitive field names match your data model properties
3. Ensure the `IDataRedactionService` is registered in DI container

### Performance Issues
1. Review regex patterns for efficiency
2. Consider using partial redaction instead of full redaction for large objects
3. Disable redaction in development if it's impacting debugging

### Custom Field Not Redacted
1. Add the field name to `SensitiveFields` configuration
2. Use case-insensitive matching by default
3. Consider partial matching if your field names contain sensitive keywords

## Testing

```csharp
[Test]
public void Should_Redact_Sensitive_Data()
{
    var options = new RedactionOptions();
    var redactionService = new DataRedactionService(options);
    
    var sensitiveData = "User password is secret123 and email is user@test.com";
    var redacted = redactionService.RedactMessage(sensitiveData);
    
    Assert.That(redacted, Does.Contain("[REDACTED]"));
    Assert.That(redacted, Does.Not.Contain("secret123"));
    Assert.That(redacted, Does.Not.Contain("user@test.com"));
}
```