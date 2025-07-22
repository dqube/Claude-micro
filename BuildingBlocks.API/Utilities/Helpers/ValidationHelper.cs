using BuildingBlocks.API.Validation.Results;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using ApiValidationResult = BuildingBlocks.API.Validation.Results.ValidationResult;

namespace BuildingBlocks.API.Utilities.Helpers;

public static class ValidationHelper
{
    public static ApiValidationResult ValidateEmail(string? email, string propertyName = "Email")
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return ApiValidationResult.Failure(propertyName, $"{propertyName} is required");
        }

        var emailAttribute = new EmailAddressAttribute();
        if (!emailAttribute.IsValid(email))
        {
            return ApiValidationResult.Failure(propertyName, $"{propertyName} is not a valid email address");
        }

        return ApiValidationResult.Success();
    }

    public static ApiValidationResult ValidatePhoneNumber(string? phone, string propertyName = "Phone")
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return ApiValidationResult.Failure(propertyName, $"{propertyName} is required");
        }

        // Simple phone validation - adjust pattern as needed
        var phonePattern = @"^[\+]?[1-9][\d]{0,15}$";
        if (!Regex.IsMatch(phone, phonePattern))
        {
            return ApiValidationResult.Failure(propertyName, $"{propertyName} is not a valid phone number");
        }

        return ApiValidationResult.Success();
    }

    public static ApiValidationResult ValidateRequired(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ApiValidationResult.Failure(propertyName, $"{propertyName} is required");
        }
        return ApiValidationResult.Success();
    }

    public static ApiValidationResult ValidateRequired<T>(T? value, string propertyName) where T : struct
    {
        if (!value.HasValue)
        {
            return ApiValidationResult.Failure(propertyName, $"{propertyName} is required");
        }
        return ApiValidationResult.Success();
    }

    public static ApiValidationResult ValidateLength(string? value, string propertyName, int minLength, int? maxLength = null)
    {
        if (string.IsNullOrEmpty(value))
        {
            return ApiValidationResult.Failure(propertyName, $"{propertyName} is required");
        }

        if (value.Length < minLength)
        {
            return ApiValidationResult.Failure(propertyName, $"{propertyName} must be at least {minLength} characters long");
        }

        if (maxLength.HasValue && value.Length > maxLength.Value)
        {
            return ApiValidationResult.Failure(propertyName, $"{propertyName} cannot be longer than {maxLength.Value} characters");
        }

        return ApiValidationResult.Success();
    }

    public static ApiValidationResult ValidateRange<T>(T value, string propertyName, T minimum, T maximum) where T : IComparable<T>
    {
        if (value.CompareTo(minimum) < 0 || value.CompareTo(maximum) > 0)
        {
            return ApiValidationResult.Failure(propertyName, $"{propertyName} must be between {minimum} and {maximum}");
        }
        return ApiValidationResult.Success();
    }

    public static ApiValidationResult ValidateGuid(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ApiValidationResult.Failure(propertyName, $"{propertyName} is required");
        }

        if (!Guid.TryParse(value, out _))
        {
            return ApiValidationResult.Failure(propertyName, $"{propertyName} is not a valid GUID");
        }

        return ApiValidationResult.Success();
    }

    public static ApiValidationResult ValidateUrl(string? url, string propertyName = "Url")
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return ApiValidationResult.Failure(propertyName, $"{propertyName} is required");
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var result) || 
            (result.Scheme != Uri.UriSchemeHttp && result.Scheme != Uri.UriSchemeHttps))
        {
            return ApiValidationResult.Failure(propertyName, $"{propertyName} is not a valid URL");
        }

        return ApiValidationResult.Success();
    }

    public static ApiValidationResult ValidateDateRange(DateTime? startDate, DateTime? endDate, string startPropertyName = "StartDate", string endPropertyName = "EndDate")
    {
        var result = ApiValidationResult.Success();

        if (!startDate.HasValue)
        {
            result += ApiValidationResult.Failure(startPropertyName, $"{startPropertyName} is required");
        }

        if (!endDate.HasValue)
        {
            result += ApiValidationResult.Failure(endPropertyName, $"{endPropertyName} is required");
        }

        if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
        {
            result += ApiValidationResult.Failure(endPropertyName, $"{endPropertyName} must be after {startPropertyName}");
        }

        return result;
    }

    public static ApiValidationResult ValidatePagination(int? page, int? pageSize, int maxPageSize = 100)
    {
        var result = ApiValidationResult.Success();

        if (page.HasValue && page.Value < 1)
        {
            result += ApiValidationResult.Failure("Page", "Page must be greater than 0");
        }

        if (pageSize.HasValue)
        {
            if (pageSize.Value < 1)
            {
                result += ApiValidationResult.Failure("PageSize", "PageSize must be greater than 0");
            }
            else if (pageSize.Value > maxPageSize)
            {
                result += ApiValidationResult.Failure("PageSize", $"PageSize cannot exceed {maxPageSize}");
            }
        }

        return result;
    }

    public static ApiValidationResult CombineResults(params ApiValidationResult[] results)
    {
        var combined = ApiValidationResult.Success();
        foreach (var result in results)
        {
            combined += result;
        }
        return combined;
    }
}