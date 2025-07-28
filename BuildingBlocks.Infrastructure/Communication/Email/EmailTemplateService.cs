using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BuildingBlocks.Infrastructure.Communication.Email;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly EmailConfiguration _configuration;
    private readonly ILogger<EmailTemplateService> _logger;
    private readonly Dictionary<string, EmailTemplate> _templateCache;

    public EmailTemplateService(
        IOptions<EmailConfiguration> configuration,
        ILogger<EmailTemplateService> logger)
    {
        _configuration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _templateCache = new Dictionary<string, EmailTemplate>();
    }

    public async Task<string> RenderTemplateAsync<T>(string templateName, T model, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(templateName);
        ArgumentNullException.ThrowIfNull(model);

        try
        {
            var template = await GetTemplateAsync(templateName, cancellationToken);
            if (template == null)
            {
                throw new InvalidOperationException($"Email template '{templateName}' not found");
            }

            var content = template.HtmlContent;
            if (string.IsNullOrEmpty(content))
            {
                content = template.TextContent;
            }

            var renderedContent = ReplaceTokens(content, model);
            _logger.LogDebug("Template '{TemplateName}' rendered successfully", templateName);
            
            return renderedContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to render email template '{TemplateName}'", templateName);
            throw;
        }
    }

    public async Task<bool> TemplateExistsAsync(string templateName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(templateName);

        try
        {
            var template = await GetTemplateAsync(templateName, cancellationToken);
            return template != null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error checking if template '{TemplateName}' exists", templateName);
            return false;
        }
    }

    public async Task<string> GetTemplateContentAsync(string templateName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(templateName);

        var template = await GetTemplateAsync(templateName, cancellationToken);
        if (template == null)
        {
            throw new InvalidOperationException($"Email template '{templateName}' not found");
        }

        return template.HtmlContent ?? template.TextContent ?? string.Empty;
    }

    private async Task<EmailTemplate?> GetTemplateAsync(string templateName, CancellationToken cancellationToken)
    {
        // Check cache first
        if (_templateCache.TryGetValue(templateName, out var cachedTemplate))
        {
            return cachedTemplate;
        }

        // Check configuration templates
        if (_configuration.Templates.Templates.TryGetValue(templateName, out var templatePath))
        {
            var template = await LoadTemplateFromFileAsync(templatePath, cancellationToken);
            if (template != null)
            {
                template.Name = templateName;
                _templateCache[templateName] = template;
                return template;
            }
        }

        // Try to load from default path
        var defaultPath = Path.Combine(_configuration.Templates.TemplatesPath, $"{templateName}.html");
        if (File.Exists(defaultPath))
        {
            var template = await LoadTemplateFromFileAsync(defaultPath, cancellationToken);
            if (template != null)
            {
                template.Name = templateName;
                _templateCache[templateName] = template;
                return template;
            }
        }

        _logger.LogWarning("Email template '{TemplateName}' not found", templateName);
        return null;
    }

    private async Task<EmailTemplate?> LoadTemplateFromFileAsync(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            var content = await File.ReadAllTextAsync(filePath, cancellationToken);
            var template = new EmailTemplate();

            // Extract subject from HTML comment if present
            var subjectMatch = Regex.Match(content, @"<!--\s*SUBJECT:\s*(.+?)\s*-->", RegexOptions.IgnoreCase);
            if (subjectMatch.Success)
            {
                template.Subject = subjectMatch.Groups[1].Value.Trim();
                content = content.Replace(subjectMatch.Value, string.Empty);
            }

            // Determine if it's HTML or text content
            if (content.TrimStart().StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase) ||
                content.TrimStart().StartsWith("<html", StringComparison.OrdinalIgnoreCase) ||
                content.Contains("<body", StringComparison.OrdinalIgnoreCase))
            {
                template.HtmlContent = content;
            }
            else
            {
                template.TextContent = content;
            }

            _logger.LogDebug("Email template loaded from file: {FilePath}", filePath);
            return template;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load email template from file: {FilePath}", filePath);
            return null;
        }
    }

    private string ReplaceTokens<T>(string content, T model)
    {
        if (string.IsNullOrEmpty(content) || model == null)
        {
            return content;
        }

        try
        {
            // Serialize model to get property values
            var json = JsonSerializer.Serialize(model, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            var modelDict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            
            if (modelDict == null)
            {
                return content;
            }

            // Replace tokens in format {{propertyName}}
            var result = content;
            foreach (var kvp in modelDict)
            {
                var token = $"{{{{{kvp.Key}}}}}";
                var value = kvp.Value?.ToString() ?? string.Empty;
                result = result.Replace(token, value, StringComparison.OrdinalIgnoreCase);
            }

            // Also support {propertyName} format
            foreach (var kvp in modelDict)
            {
                var token = $"{{{kvp.Key}}}";
                var value = kvp.Value?.ToString() ?? string.Empty;
                result = result.Replace(token, value, StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to replace tokens in template content");
            return content;
        }
    }
}