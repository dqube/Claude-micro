using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace BuildingBlocks.Infrastructure.Communication.Email;

public class SmtpEmailService : IEmailService, IDisposable
{
    private readonly EmailConfiguration _configuration;
    private readonly IEmailTemplateService _templateService;
    private readonly ILogger<SmtpEmailService> _logger;
    private readonly SmtpClient _smtpClient;
    private static readonly ActivitySource ActivitySource = new("BuildingBlocks.Infrastructure.Email");

    public SmtpEmailService(
        IOptions<EmailConfiguration> configuration,
        IEmailTemplateService templateService,
        ILogger<SmtpEmailService> logger)
    {
        _configuration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _smtpClient = CreateSmtpClient();
    }

    public async Task SendAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default)
    {
        await SendAsync(recipient, subject, body, isHtml: true, cancellationToken);
    }

    public async Task SendAsync(string recipient, string subject, string body, bool isHtml, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(recipient);
        ArgumentException.ThrowIfNullOrEmpty(subject);
        ArgumentException.ThrowIfNullOrEmpty(body);

        using var activity = ActivitySource.StartActivity("Email.Send");
        activity?.SetTag("email.provider", "smtp");
        activity?.SetTag("email.recipient_count", 1);

        try
        {
            using var message = CreateMailMessage(new[] { recipient }, subject, body, isHtml);
            
            var stopwatch = Stopwatch.StartNew();
            await _smtpClient.SendMailAsync(message, cancellationToken);
            stopwatch.Stop();

            activity?.SetTag("email.success", true);
            activity?.SetTag("email.duration_ms", stopwatch.ElapsedMilliseconds);

            _logger.LogInformation("Email sent successfully to {Recipient} with subject '{Subject}' in {Duration}ms",
                recipient, subject, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            activity?.SetTag("email.success", false);
            activity?.SetTag("exception.type", ex.GetType().Name);
            activity?.SetTag("exception.message", ex.Message);

            _logger.LogError(ex, "Failed to send email to {Recipient} with subject '{Subject}'", recipient, subject);
            throw;
        }
    }

    public async Task SendAsync(IEnumerable<string> recipients, string subject, string body, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(recipients);
        ArgumentException.ThrowIfNullOrEmpty(subject);
        ArgumentException.ThrowIfNullOrEmpty(body);

        var recipientList = recipients.ToList();
        if (!recipientList.Any())
        {
            throw new ArgumentException("Recipients list cannot be empty", nameof(recipients));
        }

        using var activity = ActivitySource.StartActivity("Email.SendBulk");
        activity?.SetTag("email.provider", "smtp");
        activity?.SetTag("email.recipient_count", recipientList.Count);

        try
        {
            using var message = CreateMailMessage(recipientList, subject, body, isHtml: true);
            
            var stopwatch = Stopwatch.StartNew();
            await _smtpClient.SendMailAsync(message, cancellationToken);
            stopwatch.Stop();

            activity?.SetTag("email.success", true);
            activity?.SetTag("email.duration_ms", stopwatch.ElapsedMilliseconds);

            _logger.LogInformation("Email sent successfully to {RecipientCount} recipients with subject '{Subject}' in {Duration}ms",
                recipientList.Count, subject, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            activity?.SetTag("email.success", false);
            activity?.SetTag("exception.type", ex.GetType().Name);
            activity?.SetTag("exception.message", ex.Message);

            _logger.LogError(ex, "Failed to send email to {RecipientCount} recipients with subject '{Subject}'",
                recipientList.Count, subject);
            throw;
        }
    }

    public async Task SendWithTemplateAsync<T>(string recipient, string templateName, T model, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(recipient);
        ArgumentException.ThrowIfNullOrEmpty(templateName);
        ArgumentNullException.ThrowIfNull(model);

        using var activity = ActivitySource.StartActivity("Email.SendWithTemplate");
        activity?.SetTag("email.provider", "smtp");
        activity?.SetTag("email.template", templateName);
        activity?.SetTag("email.recipient_count", 1);

        try
        {
            var templateExists = await _templateService.TemplateExistsAsync(templateName, cancellationToken);
            if (!templateExists)
            {
                throw new InvalidOperationException($"Email template '{templateName}' not found");
            }

            var body = await _templateService.RenderTemplateAsync(templateName, model, cancellationToken);
            
            // Extract subject from template or use template name as fallback
            var subject = ExtractSubjectFromTemplate(body) ?? $"Notification - {templateName}";
            
            await SendAsync(recipient, subject, body, isHtml: true, cancellationToken);

            _logger.LogInformation("Template email sent successfully to {Recipient} using template '{TemplateName}'",
                recipient, templateName);
        }
        catch (Exception ex)
        {
            activity?.SetTag("email.success", false);
            activity?.SetTag("exception.type", ex.GetType().Name);
            activity?.SetTag("exception.message", ex.Message);

            _logger.LogError(ex, "Failed to send template email to {Recipient} using template '{TemplateName}'",
                recipient, templateName);
            throw;
        }
    }

    public async Task SendWithAttachmentsAsync(string recipient, string subject, string body, IEnumerable<EmailAttachment> attachments, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(recipient);
        ArgumentException.ThrowIfNullOrEmpty(subject);
        ArgumentException.ThrowIfNullOrEmpty(body);
        ArgumentNullException.ThrowIfNull(attachments);

        var attachmentList = attachments.ToList();

        using var activity = ActivitySource.StartActivity("Email.SendWithAttachments");
        activity?.SetTag("email.provider", "smtp");
        activity?.SetTag("email.recipient_count", 1);
        activity?.SetTag("email.attachment_count", attachmentList.Count);

        try
        {
            using var message = CreateMailMessage(new[] { recipient }, subject, body, isHtml: true);
            
            // Add attachments
            foreach (var attachment in attachmentList)
            {
                if (attachment.Content?.Length > 0)
                {
                    var memoryStream = new MemoryStream(attachment.Content);
                    var mailAttachment = new Attachment(memoryStream, attachment.FileName, attachment.ContentType);
                    message.Attachments.Add(mailAttachment);
                }
            }

            var stopwatch = Stopwatch.StartNew();
            await _smtpClient.SendMailAsync(message, cancellationToken);
            stopwatch.Stop();

            activity?.SetTag("email.success", true);
            activity?.SetTag("email.duration_ms", stopwatch.ElapsedMilliseconds);

            _logger.LogInformation("Email with {AttachmentCount} attachments sent successfully to {Recipient} with subject '{Subject}' in {Duration}ms",
                attachmentList.Count, recipient, subject, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            activity?.SetTag("email.success", false);
            activity?.SetTag("exception.type", ex.GetType().Name);
            activity?.SetTag("exception.message", ex.Message);

            _logger.LogError(ex, "Failed to send email with attachments to {Recipient} with subject '{Subject}'",
                recipient, subject);
            throw;
        }
    }

    private SmtpClient CreateSmtpClient()
    {
        var smtpClient = new SmtpClient(_configuration.Smtp.Host, _configuration.Smtp.Port)
        {
            EnableSsl = _configuration.EnableSsl,
            Timeout = _configuration.TimeoutSeconds * 1000
        };

        if (_configuration.Smtp.UseAuthentication)
        {
            smtpClient.Credentials = new NetworkCredential(_configuration.Smtp.Username, _configuration.Smtp.Password);
        }

        _logger.LogInformation("SMTP client configured with host: {Host}, port: {Port}, SSL: {EnableSsl}",
            _configuration.Smtp.Host, _configuration.Smtp.Port, _configuration.EnableSsl);

        return smtpClient;
    }

    private MailMessage CreateMailMessage(IEnumerable<string> recipients, string subject, string body, bool isHtml)
    {
        var message = new MailMessage
        {
            From = new MailAddress(_configuration.DefaultFromAddress, _configuration.DefaultFromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml
        };

        foreach (var recipient in recipients)
        {
            message.To.Add(new MailAddress(recipient));
        }

        return message;
    }

    private static string? ExtractSubjectFromTemplate(string templateContent)
    {
        // Look for subject in HTML comment format: <!-- SUBJECT: Your Subject Here -->
        var subjectMatch = System.Text.RegularExpressions.Regex.Match(
            templateContent, 
            @"<!--\s*SUBJECT:\s*(.+?)\s*-->", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        return subjectMatch.Success ? subjectMatch.Groups[1].Value.Trim() : null;
    }

    public void Dispose()
    {
        _smtpClient?.Dispose();
        ActivitySource?.Dispose();
    }
}