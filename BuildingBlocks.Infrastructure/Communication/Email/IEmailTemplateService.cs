namespace BuildingBlocks.Infrastructure.Communication.Email;

public interface IEmailTemplateService
{
    Task<string> RenderTemplateAsync<T>(string templateName, T model, CancellationToken cancellationToken = default);
    Task<bool> TemplateExistsAsync(string templateName, CancellationToken cancellationToken = default);
    Task<string> GetTemplateContentAsync(string templateName, CancellationToken cancellationToken = default);
}

public class EmailTemplate
{
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string HtmlContent { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
}