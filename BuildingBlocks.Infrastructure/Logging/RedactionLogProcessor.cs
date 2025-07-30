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

        // Note: Due to OpenTelemetry's API limitations, we cannot directly modify
        // LogRecord.State and LogRecord.Attributes as they are read-only.
        // The primary redaction happens through the formatted message above.
        // For comprehensive redaction, use the RedactionLogger wrapper instead.

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