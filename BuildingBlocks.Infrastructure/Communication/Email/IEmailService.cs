namespace BuildingBlocks.Infrastructure.Communication.Email;

public interface IEmailService
{
    Task SendAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default);
    Task SendAsync(string recipient, string subject, string body, bool isHtml, CancellationToken cancellationToken = default);
    Task SendAsync(IEnumerable<string> recipients, string subject, string body, CancellationToken cancellationToken = default);
    Task SendWithTemplateAsync<T>(string recipient, string templateName, T model, CancellationToken cancellationToken = default);
    Task SendWithAttachmentsAsync(string recipient, string subject, string body, IEnumerable<EmailAttachment> attachments, CancellationToken cancellationToken = default);
}

public class EmailAttachment
{
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = "application/octet-stream";
}