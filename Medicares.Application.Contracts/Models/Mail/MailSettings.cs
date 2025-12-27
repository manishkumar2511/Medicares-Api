namespace Medicares.Application.Contracts.Models.Mail;

public class MailSettings
{
    public EmailSettings Email { get; set; } = new();
    public SmtpSettings Smtp { get; set; } = new();
}

public class EmailSettings
{
    public string From { get; set; } = string.Empty;
    public string ApplicationName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class SmtpSettings
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
}
