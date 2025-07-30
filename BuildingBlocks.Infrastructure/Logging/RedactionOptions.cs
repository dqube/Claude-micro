using System.Text.RegularExpressions;

namespace BuildingBlocks.Infrastructure.Logging;

public class RedactionOptions
{
    public bool Enabled { get; set; } = true;
    public string RedactionText { get; set; } = "[REDACTED]";
    
    public HashSet<string> SensitiveFields { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        // Authentication & Security
        "password",
        "pwd",
        "passwd",
        "secret",
        "token",
        "key",
        "authorization",
        "auth",
        "bearer",
        "apikey",
        "api_key",
        "accesstoken",
        "access_token",
        "refreshtoken",
        "refresh_token",
        "sessionid",
        "session_id",
        "sessiontoken",
        "session_token",
        "csrf",
        "csrftoken",
        "csrf_token",
        "xsrf",
        "xsrftoken",
        "xsrf_token",
        "privatekey",
        "private_key",
        "publickey",
        "public_key",
        "signature",
        "hash",
        "salt",
        "nonce",
        "challenge",
        "otp",
        "totp",
        "mfa",
        "2fa",
        "biometric",
        "fingerprint",
        "retina",
        "faceprint",
        
        // Financial Information
        "creditcard",
        "credit_card",
        "cardnumber",
        "card_number",
        "cardno",
        "card_no",
        "ccnumber",
        "cc_number",
        "debitcard",
        "debit_card",
        "cvv",
        "cvc",
        "cvv2",
        "cid",
        "securitycode",
        "security_code",
        "expirydate",
        "expiry_date",
        "expiration",
        "expdate",
        "exp_date",
        "accountnumber",
        "account_number",
        "routingnumber",
        "routing_number",
        "iban",
        "swift",
        "bic",
        "sortcode",
        "sort_code",
        "bankaccount",
        "bank_account",
        "banknumber",
        "bank_number",
        "creditlimit",
        "credit_limit",
        "balance",
        "accountbalance",
        "account_balance",
        "salary",
        "income",
        "wage",
        "payment",
        "transaction",
        "transactionid",
        "transaction_id",
        "invoice",
        "invoicenumber",
        "invoice_number",
        "taxid",
        "tax_id",
        "ein",
        "vat",
        "vatnumber",
        "vat_number",
        
        // Personal Identifiers
        "ssn",
        "social_security",
        "socialsecurity",
        "social_security_number",
        "tin",
        "sin",
        "nino",
        "passport",
        "passportnumber",
        "passport_number",
        "driverlicense",
        "driver_license",
        "license",
        "licensenumber",
        "license_number",
        "nationalid",
        "national_id",
        "citizenid",
        "citizen_id",
        "residentid",
        "resident_id",
        "alienid",
        "alien_id",
        "greencard",
        "green_card",
        "visa",
        "visanumber",
        "visa_number",
        "medicaid",
        "medicare",
        "healthinsurance",
        "health_insurance",
        "insuranceid",
        "insurance_id",
        "membernumber",
        "member_number",
        "memberid",
        "member_id",
        "patientid",
        "patient_id",
        "mrn",
        "medical_record_number",
        "medicalrecordnumber",
        
        // Contact Information
        "email",
        "emailaddress",
        "email_address",
        "mail",
        "e_mail",
        "phone",
        "phonenumber",
        "phone_number",
        "mobile",
        "mobilenumber",
        "mobile_number",
        "cellphone",
        "cell_phone",
        "telephone",
        "tel",
        "fax",
        "faxnumber",
        "fax_number",
        "homephone",
        "home_phone",
        "workphone",
        "work_phone",
        
        // Physical Address Information
        "address",
        "streetaddress",
        "street_address",
        "street",
        "street1",
        "street2",
        "addressline1",
        "address_line_1",
        "addressline2",
        "address_line_2",
        "city",
        "state",
        "province",
        "region",
        "county",
        "zipcode",
        "zip_code",
        "postalcode",
        "postal_code",
        "postcode",
        "post_code",
        "country",
        "countrycode",
        "country_code",
        "latitude",
        "longitude",
        "coordinates",
        "geolocation",
        "geo_location",
        "location",
        
        // Personal Details
        "firstname",
        "first_name",
        "lastname",
        "last_name",
        "fullname",
        "full_name",
        "displayname",
        "display_name",
        "nickname",
        "middlename",
        "middle_name",
        "maidenname",
        "maiden_name",
        "birthdate",
        "birth_date",
        "dob",
        "dateofbirth",
        "date_of_birth",
        "age",
        "birthyear",
        "birth_year",
        "birthplace",
        "birth_place",
        "nationality",
        "citizenship",
        "race",
        "ethnicity",
        "gender",
        "sex",
        "maritalstatus",
        "marital_status",
        "religion",
        "political",
        "politicalaffiliation",
        "political_affiliation",
        
        // Digital Identifiers
        "ip",
        "ipaddress",
        "ip_address",
        "ipv4",
        "ipv6",
        "macaddress",
        "mac_address",
        "deviceid",
        "device_id",
        "imei",
        "imsi",
        "udid",
        "uuid",
        "guid",
        "userid",
        "user_id",
        "username",
        "user_name",
        "login",
        "loginname",
        "login_name",
        "screenname",
        "screen_name",
        "handle",
        "alias",
        "customerid",
        "customer_id",
        "clientid",
        "client_id",
        "employeeid",
        "employee_id",
        "staffid",
        "staff_id",
        "vendorid",
        "vendor_id",
        "supplierid",
        "supplier_id",
        
        // Biometric & Health Data
        "biometric",
        "fingerprint",
        "retina",
        "iris",
        "faceprint",
        "voiceprint",
        "dna",
        "genetic",
        "bloodtype",
        "blood_type",
        "medicalhistory",
        "medical_history",
        "diagnosis",
        "medication",
        "prescription",
        "allergy",
        "allergies",
        "disability",
        "mentalhealth",
        "mental_health",
        "chroniccondition",
        "chronic_condition",
        "treatment",
        "surgery",
        "procedure",
        "vaccine",
        "vaccination",
        "immunization",
        
        // Educational & Professional
        "studentid",
        "student_id",
        "schoolid",
        "school_id",
        "graduationyear",
        "graduation_year",
        "gpa",
        "grade",
        "transcript",
        "diploma",
        "degree",
        "certification",
        "license",
        "profession",
        "occupation",
        "employer",
        "workplace",
        "jobTitle",
        "job_title",
        "department",
        "position",
        "rank",
        "level",
        "clearance",
        "securityclearance",
        "security_clearance",
        
        // Family & Relationships
        "mothername",
        "mother_name",
        "fathername",
        "father_name",
        "spousename",
        "spouse_name",
        "children",
        "childname",
        "child_name",
        "familyname",
        "family_name",
        "relative",
        "relationship",
        "emergencycontact",
        "emergency_contact",
        "nextofkin",
        "next_of_kin",
        
        // Generic PII Categories
        "personaldata",
        "personal_data",
        "pii",
        "phi",
        "sensitive",
        "confidential",
        "restricted",
        "classified",
        "private",
        "internal",
        "proprietary",
        "personallyidentifiableinformation",
        "personally_identifiable_information",
        "protectedhealthinformation",
        "protected_health_information",
        "sensitivedata",
        "sensitive_data",
        "personalinformation",
        "personal_information",
        "privatedata",
        "private_data",
        "confidentialdata",
        "confidential_data"
    };

    public Dictionary<string, Regex> RegexPatterns { get; set; } = new()
    {
        // Email addresses
        ["email"] = new Regex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        
        // Credit card numbers (various formats)
        ["creditcard"] = new Regex(@"\b(?:\d{4}[-\s]?\d{4}[-\s]?\d{4}[-\s]?\d{4}|\d{13,19})\b", RegexOptions.Compiled),
        ["amex"] = new Regex(@"\b3[47]\d{13}\b", RegexOptions.Compiled),
        ["visa"] = new Regex(@"\b4\d{15}\b", RegexOptions.Compiled),
        ["mastercard"] = new Regex(@"\b5[1-5]\d{14}\b", RegexOptions.Compiled),
        ["discover"] = new Regex(@"\b6(?:011|5\d{2})\d{12}\b", RegexOptions.Compiled),
        
        // Social Security Numbers (various formats)
        ["ssn"] = new Regex(@"\b\d{3}[-\s]?\d{2}[-\s]?\d{4}\b", RegexOptions.Compiled),
        ["ssn_full"] = new Regex(@"\b\d{9}\b", RegexOptions.Compiled),
        
        // Tax ID Numbers
        ["ein"] = new Regex(@"\b\d{2}-\d{7}\b", RegexOptions.Compiled),
        ["itin"] = new Regex(@"\b9\d{2}[-\s]?\d{2}[-\s]?\d{4}\b", RegexOptions.Compiled),
        
        // Phone numbers (international formats)
        ["phone_us"] = new Regex(@"\b(\+?1[-.\s]?)?\(?[0-9]{3}\)?[-.\s]?[0-9]{3}[-.\s]?[0-9]{4}\b", RegexOptions.Compiled),
        ["phone_international"] = new Regex(@"\+\d{1,3}[-.\s]?\d{1,14}", RegexOptions.Compiled),
        ["phone_generic"] = new Regex(@"\b\d{3}[-.\s]?\d{3}[-.\s]?\d{4}\b", RegexOptions.Compiled),
        
        // IP addresses (IPv4 and IPv6)
        ["ipv4"] = new Regex(@"\b(?:[0-9]{1,3}\.){3}[0-9]{1,3}\b", RegexOptions.Compiled),
        ["ipv6"] = new Regex(@"\b(?:[0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}\b", RegexOptions.Compiled),
        ["ipv6_compressed"] = new Regex(@"\b(?:[0-9a-fA-F]{1,4}:)*::(?:[0-9a-fA-F]{1,4}:)*[0-9a-fA-F]{1,4}\b", RegexOptions.Compiled),
        
        // MAC addresses
        ["mac_address"] = new Regex(@"\b(?:[0-9a-fA-F]{2}[:-]){5}[0-9a-fA-F]{2}\b", RegexOptions.Compiled),
        
        // Driver's License patterns (US states)
        ["drivers_license"] = new Regex(@"\b[A-Z]{1,2}\d{6,8}\b", RegexOptions.Compiled),
        
        // Passport numbers
        ["passport_us"] = new Regex(@"\b\d{9}\b", RegexOptions.Compiled),
        ["passport_international"] = new Regex(@"\b[A-Z]{1,2}\d{6,9}\b", RegexOptions.Compiled),
        
        // Bank account numbers
        ["bank_account"] = new Regex(@"\b\d{8,17}\b", RegexOptions.Compiled),
        ["routing_number"] = new Regex(@"\b\d{9}\b", RegexOptions.Compiled),
        ["iban"] = new Regex(@"\b[A-Z]{2}\d{2}[A-Z0-9]{4}\d{7}[A-Z0-9]{0,16}\b", RegexOptions.Compiled),
        
        // Insurance numbers
        ["medicare"] = new Regex(@"\b\d{3}-\d{2}-\d{4}[A-Z]?\b", RegexOptions.Compiled),
        ["medicaid"] = new Regex(@"\b\d{8,12}\b", RegexOptions.Compiled),
        
        // API keys and tokens (various patterns)
        ["api_key"] = new Regex(@"\b[A-Za-z0-9]{32,}\b", RegexOptions.Compiled),
        ["aws_key"] = new Regex(@"\bAKIA[0-9A-Z]{16}\b", RegexOptions.Compiled),
        ["jwt"] = new Regex(@"\beyJ[A-Za-z0-9_-]*\.[A-Za-z0-9_-]*\.[A-Za-z0-9_-]*\b", RegexOptions.Compiled),
        ["bearer_token"] = new Regex(@"\bBearer\s+[A-Za-z0-9_-]+", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        ["github_token"] = new Regex(@"\bghp_[A-Za-z0-9]{36}\b", RegexOptions.Compiled),
        ["slack_token"] = new Regex(@"\bxox[baprs]-[A-Za-z0-9-]+", RegexOptions.Compiled),
        
        // UUIDs and GUIDs
        ["uuid"] = new Regex(@"\b[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}\b", RegexOptions.Compiled),
        ["guid"] = new Regex(@"\b[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}\b", RegexOptions.Compiled),
        
        // Coordinates (latitude/longitude)
        ["coordinates"] = new Regex(@"-?\d{1,3}\.\d+,\s*-?\d{1,3}\.\d+", RegexOptions.Compiled),
        ["latitude"] = new Regex(@"\b-?(?:90|[1-8]?\d(?:\.\d+)?)\b", RegexOptions.Compiled),
        ["longitude"] = new Regex(@"\b-?(?:180|1[0-7]\d|[1-9]?\d(?:\.\d+)?)\b", RegexOptions.Compiled),
        
        // Date of birth patterns
        ["date_of_birth"] = new Regex(@"\b(?:0[1-9]|1[0-2])[-/](?:0[1-9]|[12]\d|3[01])[-/](?:19|20)\d{2}\b", RegexOptions.Compiled),
        ["dob_yyyy"] = new Regex(@"\b(?:19|20)\d{2}[-/](?:0[1-9]|1[0-2])[-/](?:0[1-9]|[12]\d|3[01])\b", RegexOptions.Compiled),
        
        // Medical Record Numbers
        ["mrn"] = new Regex(@"\bMRN[-:]?\s*\d{6,10}\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        ["patient_id"] = new Regex(@"\bPID[-:]?\s*\d{6,10}\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        
        // Vehicle identification
        ["vin"] = new Regex(@"\b[A-HJ-NPR-Z0-9]{17}\b", RegexOptions.Compiled),
        ["license_plate"] = new Regex(@"\b[A-Z0-9]{2,8}\b", RegexOptions.Compiled),
        
        // Biometric patterns (simplified)
        ["fingerprint_template"] = new Regex(@"\b[A-Fa-f0-9]{64,}\b", RegexOptions.Compiled),
        
        // Database connection strings
        ["connection_string"] = new Regex(@"(?:Server|Data Source|Initial Catalog|User ID|Password|pwd)\s*=\s*[^;]+", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        
        // Basic password patterns in logs
        ["password_field"] = new Regex(@"(password|pwd|secret|token|key)\s*[:=]\s*[^\s,}]+", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        
        // Common hash patterns
        ["md5_hash"] = new Regex(@"\b[a-fA-F0-9]{32}\b", RegexOptions.Compiled),
        ["sha1_hash"] = new Regex(@"\b[a-fA-F0-9]{40}\b", RegexOptions.Compiled),
        ["sha256_hash"] = new Regex(@"\b[a-fA-F0-9]{64}\b", RegexOptions.Compiled),
        
        // URL with credentials
        ["url_with_credentials"] = new Regex(@"https?://[^:]+:[^@]+@[^\s]+", RegexOptions.Compiled | RegexOptions.IgnoreCase)
    };

    public RedactionMode Mode { get; set; } = RedactionMode.Full;
    
    public Dictionary<string, RedactionStrategy> FieldStrategies { get; set; } = new()
    {
        // Contact Information - Partial masking to maintain some utility
        ["email"] = RedactionStrategy.PartialMask,
        ["phone"] = RedactionStrategy.PartialMask,
        ["address"] = RedactionStrategy.PartialMask,
        
        // Financial Data - Partial masking for last 4 digits
        ["creditcard"] = RedactionStrategy.PartialMask,
        ["accountnumber"] = RedactionStrategy.PartialMask,
        ["routingnumber"] = RedactionStrategy.PartialMask,
        
        // Authentication & Security - Full masking for security
        ["password"] = RedactionStrategy.FullMask,
        ["secret"] = RedactionStrategy.FullMask,
        ["token"] = RedactionStrategy.FullMask,
        ["apikey"] = RedactionStrategy.FullMask,
        ["privatekey"] = RedactionStrategy.FullMask,
        ["signature"] = RedactionStrategy.FullMask,
        ["hash"] = RedactionStrategy.FullMask,
        ["salt"] = RedactionStrategy.FullMask,
        
        // Government IDs - Full masking for privacy
        ["ssn"] = RedactionStrategy.FullMask,
        ["passport"] = RedactionStrategy.FullMask,
        ["driverlicense"] = RedactionStrategy.FullMask,
        ["nationalid"] = RedactionStrategy.FullMask,
        
        // Medical Information - Full masking for HIPAA compliance
        ["mrn"] = RedactionStrategy.FullMask,
        ["patientid"] = RedactionStrategy.FullMask,
        ["diagnosis"] = RedactionStrategy.FullMask,
        ["medication"] = RedactionStrategy.FullMask,
        ["medicalhistory"] = RedactionStrategy.FullMask,
        
        // Biometric Data - Full masking
        ["fingerprint"] = RedactionStrategy.FullMask,
        ["biometric"] = RedactionStrategy.FullMask,
        ["retina"] = RedactionStrategy.FullMask,
        ["dna"] = RedactionStrategy.FullMask,
        
        // Personal Details - Mixed strategies
        ["firstname"] = RedactionStrategy.PartialMask,
        ["lastname"] = RedactionStrategy.PartialMask,
        ["fullname"] = RedactionStrategy.PartialMask,
        ["birthdate"] = RedactionStrategy.FullMask,
        ["age"] = RedactionStrategy.Length,
        
        // System Identifiers - Hash for consistency
        ["userid"] = RedactionStrategy.Hash,
        ["customerid"] = RedactionStrategy.Hash,
        ["employeeid"] = RedactionStrategy.Hash,
        ["uuid"] = RedactionStrategy.Hash,
        ["guid"] = RedactionStrategy.Hash,
        
        // Network Information - Partial masking
        ["ip"] = RedactionStrategy.PartialMask,
        ["macaddress"] = RedactionStrategy.PartialMask,
        
        // Generic sensitive categories - Full masking by default
        ["pii"] = RedactionStrategy.FullMask,
        ["phi"] = RedactionStrategy.FullMask,
        ["sensitive"] = RedactionStrategy.FullMask,
        ["confidential"] = RedactionStrategy.FullMask,
        ["personaldata"] = RedactionStrategy.FullMask
    };
}

public enum RedactionMode
{
    Disabled = 0,
    Full = 1,
    Partial = 2,
    Custom = 3
}

public enum RedactionStrategy
{
    FullMask,      // [REDACTED]
    PartialMask,   // Show some characters (e.g., first/last few)
    Hash,          // Show hash of the value
    Length         // Show length only
}