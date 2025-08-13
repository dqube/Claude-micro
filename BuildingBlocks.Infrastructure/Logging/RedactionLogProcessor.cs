using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace BuildingBlocks.Infrastructure.Logging;

public class RedactionLogProcessor : BaseProcessor<LogRecord>
{
    private readonly IDataRedactionService _redactionService;

    public RedactionLogProcessor(IDataRedactionService redactionService)
    {
        _redactionService = redactionService ?? throw new ArgumentNullException(nameof(redactionService));
    }

    public override void OnEnd(LogRecord data)
    {
        ArgumentNullException.ThrowIfNull(data);

        // Use the new Attributes property instead of deprecated State
        if (data.Attributes != null)
        {
            var redactedAttributes = new RedactionAttributesEnumerator(data.Attributes, _redactionService);
            data.Attributes = redactedAttributes;
            
            // Try to recreate the formatted message with redacted parameters
            try
            {
                var template = data.Attributes.FirstOrDefault(kvp => kvp.Key == "{OriginalFormat}").Value as string;
                if (!string.IsNullOrEmpty(template))
                {
                    // Extract parameter values from redacted attributes
                    var parameters = new List<object?>();
                    foreach (var kvp in data.Attributes.Where(x => x.Key != "{OriginalFormat}"))
                    {
                        parameters.Add(kvp.Value);
                    }
                    
                    // Reconstruct the formatted message with redacted values using MessageTemplate logic
                    if (parameters.Count > 0)
                    {
                        try
                        {
                            // Use a simple template replacement approach
                            var formattedMessage = template;
                            var parameterIndex = 0;
                            
                            // Replace {ParameterName} with redacted values
                            foreach (var kvp in data.Attributes.Where(x => x.Key != "{OriginalFormat}"))
                            {
                                var parameterName = kvp.Key;
                                var parameterValue = kvp.Value?.ToString() ?? "null";
                                
                                // Replace both {ParameterName} and {index} patterns
                                formattedMessage = formattedMessage.Replace($"{{{parameterName}}}", parameterValue, StringComparison.OrdinalIgnoreCase);
                                formattedMessage = formattedMessage.Replace($"{{{parameterIndex}}}", parameterValue);
                                parameterIndex++;
                            }
                            
                            data.FormattedMessage = formattedMessage;
                        }
                        catch
                        {
                            // If custom formatting fails, fall back to redacting the original message
                            if (!string.IsNullOrEmpty(data.FormattedMessage))
                            {
                                data.FormattedMessage = _redactionService.RedactMessage(data.FormattedMessage);
                            }
                        }
                    }
                }
            }
            catch
            {
                // If attribute processing fails, ensure we still have basic redaction
                if (!string.IsNullOrEmpty(data.FormattedMessage))
                {
                    data.FormattedMessage = _redactionService.RedactMessage(data.FormattedMessage);
                }
            }
        }
        else
        {
            // No attributes, just redact the formatted message
            if (!string.IsNullOrEmpty(data.FormattedMessage))
            {
                data.FormattedMessage = _redactionService.RedactMessage(data.FormattedMessage);
            }
        }

        // Redact log record body if it's a string
        if (data.Body is string bodyString)
        {
            data.Body = _redactionService.RedactMessage(bodyString);
        }

        base.OnEnd(data);
    }
}

internal sealed class RedactionAttributesEnumerator : IReadOnlyList<KeyValuePair<string, object?>>
{
    private readonly IReadOnlyList<KeyValuePair<string, object?>> _attributes;
    private readonly IDataRedactionService _redactionService;

    public RedactionAttributesEnumerator(IReadOnlyList<KeyValuePair<string, object?>> attributes, IDataRedactionService redactionService)
    {
        _attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
        _redactionService = redactionService ?? throw new ArgumentNullException(nameof(redactionService));
    }

    public int Count => _attributes.Count;

    public KeyValuePair<string, object?> this[int index]
    {
        get
        {
            var item = _attributes[index];
            
            // Check if this attribute should be redacted based on key name
            if (_redactionService.IsSensitiveField(item.Key))
            {
                return new KeyValuePair<string, object?>(item.Key, _redactionService.RedactMessage(item.Value?.ToString() ?? string.Empty));
            }
            
            // Check if the value contains sensitive data
            var valueStr = item.Value?.ToString();
            if (!string.IsNullOrEmpty(valueStr) && _redactionService.ContainsSensitiveData(valueStr))
            {
                return new KeyValuePair<string, object?>(item.Key, _redactionService.RedactMessage(valueStr));
            }

            return item;
        }
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class RedactionActivityProcessor : BaseProcessor<Activity>
{
    private readonly IDataRedactionService _redactionService;

    public RedactionActivityProcessor(IDataRedactionService redactionService)
    {
        _redactionService = redactionService ?? throw new ArgumentNullException(nameof(redactionService));
    }

    public override void OnEnd(Activity data)
    {
        ArgumentNullException.ThrowIfNull(data);
        // Redact activity display name
        if (!string.IsNullOrEmpty(data.DisplayName))
        {
            var redactedDisplayName = _redactionService.RedactMessage(data.DisplayName);
            if (redactedDisplayName != data.DisplayName)
            {
                data.DisplayName = redactedDisplayName;
            }
        }

        // Redact tags
        var tagsToUpdate = new List<KeyValuePair<string, string?>>();
        foreach (var tag in data.Tags)
        {
            if (!string.IsNullOrEmpty(tag.Value))
            {
                var redactedValue = _redactionService.RedactMessage(tag.Value);
                if (redactedValue != tag.Value)
                {
                    tagsToUpdate.Add(new KeyValuePair<string, string?>(tag.Key, redactedValue));
                }
            }
        }

        // Update tags that need redaction
        foreach (var tag in tagsToUpdate)
        {
            data.SetTag(tag.Key, tag.Value);
        }

        // Redact baggage
        var baggageToUpdate = new List<KeyValuePair<string, string?>>();
        foreach (var baggage in data.Baggage)
        {
            if (!string.IsNullOrEmpty(baggage.Value))
            {
                var redactedValue = _redactionService.RedactMessage(baggage.Value);
                if (redactedValue != baggage.Value)
                {
                    baggageToUpdate.Add(new KeyValuePair<string, string?>(baggage.Key, redactedValue));
                }
            }
        }

        // Update baggage that needs redaction
        foreach (var baggage in baggageToUpdate)
        {
            data.SetBaggage(baggage.Key, baggage.Value);
        }

        base.OnEnd(data);
    }
}