namespace Medicares.Application.Contracts.Models.Mail;

public class MailSettings
{
    public EmailSettings Email { get; set; } = new();
}

public class EmailSettings
{
    public string From { get; set; } = string.Empty;
    public string ApplicationName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
