using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BuildingBlocks.API.Utilities.Helpers;

public static class ValidationHelper
{
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var emailAttribute = new EmailAddressAttribute();
            return emailAttribute.IsValid(email);
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out var result)
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    public static bool IsValidGuid(string guidString)
    {
        return Guid.TryParse(guidString, out _);
    }

    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Basic phone number validation - can be enhanced based on requirements
        var phonePattern = @"^[\+]?[1-9][\d]{0,15}$";
        return Regex.IsMatch(phoneNumber, phonePattern);
    }

    public static bool IsStrongPassword(string password, int minLength = 8)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < minLength)
            return false;

        bool hasUpper = password.Any(char.IsUpper);
        bool hasLower = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }

    public static bool IsValidDateRange(DateTime startDate, DateTime endDate)
    {
        return startDate <= endDate;
    }

    public static bool IsInRange(int value, int min, int max)
    {
        return value >= min && value <= max;
    }

    public static bool IsInRange(decimal value, decimal min, decimal max)
    {
        return value >= min && value <= max;
    }

    public static string SanitizeInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove potentially dangerous characters
        input = input.Trim();
        input = Regex.Replace(input, @"[<>""'%;()&+]", "");
        
        return input;
    }

    public static bool HasValidLength(string value, int minLength, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return minLength == 0;

        return value.Length >= minLength && value.Length <= maxLength;
    }

    public static bool ContainsOnlyAlphanumeric(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        return value.All(char.IsLetterOrDigit);
    }

    public static bool IsValidPageSize(int pageSize, int maxPageSize = 100)
    {
        return pageSize > 0 && pageSize <= maxPageSize;
    }

    public static bool IsValidPageNumber(int pageNumber)
    {
        return pageNumber > 0;
    }
}