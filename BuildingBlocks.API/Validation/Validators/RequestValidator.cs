using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.API.Validation.Validators;

public static class RequestValidator
{
    public static BuildingBlocks.API.Validation.Results.ValidationResult ValidateObject(object obj)
    {
        var validationContext = new ValidationContext(obj);
        var dataAnnotationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        
        bool isValid = Validator.TryValidateObject(obj, validationContext, dataAnnotationResults, true);
        
        if (isValid)
            return BuildingBlocks.API.Validation.Results.ValidationResult.Success();

        var errors = dataAnnotationResults
            .SelectMany(vr => vr.MemberNames.Select(memberName => 
                new BuildingBlocks.API.Validation.Results.ValidationError(memberName, vr.ErrorMessage ?? "Validation error")))
            .ToArray();

        return BuildingBlocks.API.Validation.Results.ValidationResult.Failure(errors);
    }

    public static async Task<BuildingBlocks.API.Validation.Results.ValidationResult> ValidateJsonAsync<T>(HttpRequest request) where T : class
    {
        try
        {
            if (!request.HasJsonContentType())
            {
                return BuildingBlocks.API.Validation.Results.ValidationResult.Failure("ContentType", "Request must have JSON content type");
            }

            using var reader = new StreamReader(request.Body);
            var json = await reader.ReadToEndAsync();
            
            if (string.IsNullOrWhiteSpace(json))
            {
                return BuildingBlocks.API.Validation.Results.ValidationResult.Failure("Body", "Request body cannot be empty");
            }

            var obj = JsonSerializer.Deserialize<T>(json);
            if (obj == null)
            {
                return BuildingBlocks.API.Validation.Results.ValidationResult.Failure("Body", "Invalid JSON format");
            }

            return ValidateObject(obj);
        }
        catch (JsonException ex)
        {
            return BuildingBlocks.API.Validation.Results.ValidationResult.Failure("Body", $"Invalid JSON: {ex.Message}");
        }
        catch (Exception ex)
        {
            return BuildingBlocks.API.Validation.Results.ValidationResult.Failure("Request", $"Validation error: {ex.Message}");
        }
    }

    public static BuildingBlocks.API.Validation.Results.ValidationResult ValidateRequired(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return BuildingBlocks.API.Validation.Results.ValidationResult.Failure(propertyName, $"{propertyName} is required");
        }
        return BuildingBlocks.API.Validation.Results.ValidationResult.Success();
    }

    public static BuildingBlocks.API.Validation.Results.ValidationResult ValidateEmail(string? email, string propertyName = "Email")
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BuildingBlocks.API.Validation.Results.ValidationResult.Failure(propertyName, $"{propertyName} is required");
        }

        if (!IsValidEmail(email))
        {
            return BuildingBlocks.API.Validation.Results.ValidationResult.Failure(propertyName, $"{propertyName} is not a valid email address");
        }

        return BuildingBlocks.API.Validation.Results.ValidationResult.Success();
    }

    public static BuildingBlocks.API.Validation.Results.ValidationResult ValidateLength(string? value, string propertyName, int minLength, int? maxLength = null)
    {
        if (string.IsNullOrEmpty(value))
        {
            return BuildingBlocks.API.Validation.Results.ValidationResult.Failure(propertyName, $"{propertyName} is required");
        }

        if (value.Length < minLength)
        {
            return BuildingBlocks.API.Validation.Results.ValidationResult.Failure(propertyName, $"{propertyName} must be at least {minLength} characters long");
        }

        if (maxLength.HasValue && value.Length > maxLength.Value)
        {
            return BuildingBlocks.API.Validation.Results.ValidationResult.Failure(propertyName, $"{propertyName} cannot be longer than {maxLength.Value} characters");
        }

        return BuildingBlocks.API.Validation.Results.ValidationResult.Success();
    }

    private static bool IsValidEmail(string email)
    {
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
}