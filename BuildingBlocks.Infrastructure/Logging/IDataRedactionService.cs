using System.Collections;
using System.Reflection;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Logging;

public interface IDataRedactionService
{
    string RedactMessage(string message);
    string RedactJson(string json);
    string RedactObject(object obj);
    Dictionary<string, object?> RedactProperties(Dictionary<string, object?> properties);
    bool IsSensitiveField(string fieldName);
    bool ContainsSensitiveData(string data);
}

public class DataRedactionService : IDataRedactionService
{
    private readonly RedactionOptions _options;
    private readonly Dictionary<string, Func<string, string>> _customRedactors;

    public DataRedactionService(RedactionOptions options)
    {
        _options = options ?? new RedactionOptions();
        _customRedactors = new Dictionary<string, Func<string, string>>(StringComparer.OrdinalIgnoreCase);
        InitializeDefaultRedactors();
    }

    public string RedactMessage(string message)
    {
        if (string.IsNullOrEmpty(message) || !_options.Enabled)
            return message;

        var redactedMessage = message;

        // Apply regex patterns
        foreach (var pattern in _options.RegexPatterns)
        {
            redactedMessage = pattern.Value.Replace(redactedMessage, _options.RedactionText);
        }

        // Apply field-based redaction for structured messages
        foreach (var field in _options.SensitiveFields)
        {
            redactedMessage = RedactFieldInMessage(redactedMessage, field);
        }

        return redactedMessage;
    }

    public string RedactJson(string json)
    {
        if (string.IsNullOrEmpty(json) || !_options.Enabled)
            return json;

        try
        {
            var jsonDoc = JsonDocument.Parse(json);
            var redactedJson = RedactJsonElement(jsonDoc.RootElement);
            return JsonSerializer.Serialize(redactedJson, new JsonSerializerOptions { WriteIndented = false });
        }
        catch
        {
            // If JSON parsing fails, fall back to message redaction
            return RedactMessage(json);
        }
    }

    public string RedactObject(object obj)
    {
        if (obj == null || !_options.Enabled)
            return obj?.ToString() ?? string.Empty;

        try
        {
            // If it's a collection, handle each item
            if (obj is IEnumerable enumerable && !(obj is string))
            {
                var items = new List<object>();
                foreach (var item in enumerable)
                {
                    if (item != null)
                    {
                        var itemStr = item.ToString() ?? string.Empty;
                        if (ContainsSensitiveData(itemStr) || HasSensitiveProperties(item))
                        {
                            var json = JsonSerializer.Serialize(item, new JsonSerializerOptions { WriteIndented = false });
                            items.Add(RedactJson(json));
                        }
                        else
                        {
                            items.Add(item);
                        }
                    }
                    else
                    {
                        items.Add(item);
                    }
                }
                return string.Join(", ", items);
            }
            
            // For single objects, serialize to JSON and redact
            var objStr = obj.ToString() ?? string.Empty;
            if (ContainsSensitiveData(objStr) || HasSensitiveProperties(obj))
            {
                var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = false });
                return RedactJson(json);
            }
            
            return objStr;
        }
        catch
        {
            return RedactMessage(obj.ToString() ?? string.Empty);
        }
    }

    private bool HasSensitiveProperties(object obj)
    {
        if (obj == null) return false;
        
        var type = obj.GetType();
        var properties = type.GetProperties();
        
        foreach (var prop in properties)
        {
            if (IsSensitiveField(prop.Name))
            {
                return true;
            }
        }
        
        return false;
    }

    public Dictionary<string, object?> RedactProperties(Dictionary<string, object?> properties)
    {
        if (properties == null || !_options.Enabled)
            return properties ?? new Dictionary<string, object?>();

        var redactedProperties = new Dictionary<string, object?>();

        foreach (var kvp in properties)
        {
            if (IsSensitiveField(kvp.Key))
            {
                redactedProperties[kvp.Key] = _options.RedactionText;
            }
            else if (kvp.Value is string stringValue)
            {
                redactedProperties[kvp.Key] = RedactMessage(stringValue);
            }
            else if (kvp.Value != null)
            {
                redactedProperties[kvp.Key] = RedactObject(kvp.Value);
            }
            else
            {
                redactedProperties[kvp.Key] = kvp.Value;
            }
        }

        return redactedProperties;
    }

    private void InitializeDefaultRedactors()
    {
        // Email redaction
        _customRedactors["email"] = value => 
        {
            var atIndex = value.IndexOf('@');
            if (atIndex > 1)
            {
                return value[0] + "***" + value.Substring(atIndex);
            }
            return _options.RedactionText;
        };

        // Credit card redaction (show last 4 digits)
        _customRedactors["creditcard"] = value =>
        {
            if (value.Length >= 4)
            {
                return "****-****-****-" + value.Substring(value.Length - 4);
            }
            return _options.RedactionText;
        };

        // Phone number redaction
        _customRedactors["phone"] = value =>
        {
            if (value.Length >= 4)
            {
                return "***-***-" + value.Substring(value.Length - 4);
            }
            return _options.RedactionText;
        };
    }

    private object RedactJsonElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var obj = new Dictionary<string, object?>();
                foreach (var property in element.EnumerateObject())
                {
                    if (IsSensitiveField(property.Name))
                    {
                        obj[property.Name] = _options.RedactionText;
                    }
                    else
                    {
                        obj[property.Name] = RedactJsonElement(property.Value);
                    }
                }
                return obj;

            case JsonValueKind.Array:
                var array = new List<object?>();
                foreach (var item in element.EnumerateArray())
                {
                    array.Add(RedactJsonElement(item));
                }
                return array;

            case JsonValueKind.String:
                var stringValue = element.GetString() ?? string.Empty;
                return RedactMessage(stringValue);

            default:
                return element.GetRawText();
        }
    }

    private string RedactFieldInMessage(string message, string fieldName)
    {
        // Look for patterns like "fieldName": "value" or fieldName=value
        var patterns = new[]
        {
            $@"(""{fieldName}""\s*:\s*"")[^""]*(""|$)",
            $@"({fieldName}\s*=\s*"")[^""]*(""|$)",
            $@"({fieldName}\s*=\s*)[^\s,}}]*",
        };

        foreach (var pattern in patterns)
        {
            message = System.Text.RegularExpressions.Regex.Replace(
                message, 
                pattern, 
                $"$1{_options.RedactionText}$2",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        return message;
    }

    public bool IsSensitiveField(string fieldName)
    {
        return _options.SensitiveFields.Any(sf => 
            string.Equals(sf, fieldName, StringComparison.OrdinalIgnoreCase) ||
            fieldName.Contains(sf, StringComparison.OrdinalIgnoreCase));
    }

    public bool ContainsSensitiveData(string data)
    {
        if (string.IsNullOrEmpty(data))
            return false;

        // Check if the data contains any sensitive field patterns
        foreach (var field in _options.SensitiveFields)
        {
            if (data.Contains(field, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        // Check against regex patterns if configured
        foreach (var pattern in _options.RegexPatterns)
        {
            if (pattern.Value.IsMatch(data))
                return true;
        }

        return false;
    }
}