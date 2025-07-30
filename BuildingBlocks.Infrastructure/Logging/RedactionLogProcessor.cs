using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
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
        // Redact the formatted message
        if (!string.IsNullOrEmpty(data.FormattedMessage))
        {
            data.FormattedMessage = _redactionService.RedactMessage(data.FormattedMessage);
        }

        // Redact log record body if it's a string
        if (data.Body is string bodyString)
        {
            data.Body = _redactionService.RedactMessage(bodyString);
        }

        // Redact state values if they exist
        if (data.State is IReadOnlyList<KeyValuePair<string, object?>> stateList)
        {
            var redactedState = new List<KeyValuePair<string, object?>>();
            foreach (var kvp in stateList)
            {
                var redactedValue = kvp.Value;
                if (kvp.Value is string stringValue)
                {
                    redactedValue = _redactionService.RedactMessage(stringValue);
                }
                else if (kvp.Value != null)
                {
                    var jsonValue = _redactionService.RedactObject(kvp.Value);
                    redactedValue = jsonValue;
                }

                redactedState.Add(new KeyValuePair<string, object?>(kvp.Key, redactedValue));
            }
            // Note: LogRecord.State is read-only, so we can't modify it directly
            // This is a limitation of OpenTelemetry's current API
        }

        // Redact attributes
        if (data.Attributes != null)
        {
            var attributesToUpdate = new List<KeyValuePair<string, object?>>();
            
            foreach (var attribute in data.Attributes)
            {
                if (attribute.Value is string stringValue)
                {
                    var redactedValue = _redactionService.RedactMessage(stringValue);
                    if (redactedValue != stringValue)
                    {
                        attributesToUpdate.Add(new KeyValuePair<string, object?>(attribute.Key, redactedValue));
                    }
                }
                else if (attribute.Value != null)
                {
                    var redactedValue = _redactionService.RedactObject(attribute.Value);
                    if (redactedValue != attribute.Value?.ToString())
                    {
                        attributesToUpdate.Add(new KeyValuePair<string, object?>(attribute.Key, redactedValue));
                    }
                }
            }

            // Update attributes that need redaction
            foreach (var kvp in attributesToUpdate)
            {
                data.Attributes?.Add(kvp);
            }
        }

        base.OnEnd(data);
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