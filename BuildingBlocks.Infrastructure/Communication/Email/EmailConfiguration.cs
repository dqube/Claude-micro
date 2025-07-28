namespace BuildingBlocks.Infrastructure.Communication.Email;

public class EmailConfiguration
{
    public string Provider { get; set; } = "Smtp"; // Smtp, SendGrid, etc.
    public SmtpConfiguration Smtp { get; set; } = new();
    public SendGridConfiguration SendGrid { get; set; } = new();
    public TemplateConfiguration Templates { get; set; } = new();
    public string DefaultFromAddress { get; set; } = string.Empty;
    public string DefaultFromName { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
    public int TimeoutSeconds { get; set; } = 30;
}

public class SmtpConfiguration
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UseAuthentication { get; set; } = true;
}

public class SendGridConfiguration
{
    public string ApiKey { get; set; } = string.Empty;
    public string SandboxMode { get; set; } = "false";
}

public class TemplateConfiguration
{
    public string TemplatesPath { get; set; } = "EmailTemplates";
    public Dictionary<string, string> Templates { get; set; } = new();
}