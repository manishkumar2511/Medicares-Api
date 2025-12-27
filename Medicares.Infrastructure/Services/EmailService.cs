using Medicares.Application.Contracts.Interfaces;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Application.Contracts.Models.Mail;
using Medicares.Domain.Shared.Constant;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Medicares.Infrastructure.Services;

public sealed class EmailService : IEmailService
{
    private readonly MailSettings _settings;
    private readonly IConfiguration _configuration;

    public EmailService(
        IOptions<MailSettings> mailSettings,
        IConfiguration configuration)
    {
        _settings = mailSettings.Value;
        _configuration = configuration;
    }

    public async Task<bool> SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.Smtp.Host))
        {
            return true;
        }

        try
        {
            using SmtpClient smtpClient = CreateSmtpClient();
            using MailMessage mailMessage = CreateMailMessage(to, subject, body);

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);
            return true;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Email send failed: {exception}");
            return false;
        }
    }

    public async Task<bool> SendWelcomeEmailAsync(
        string email,
        string fullName,
        CancellationToken cancellationToken = default)
    {
        string? templatePath = ResolveTemplatePath("EmailTemplate.html");

        if (templatePath is null)
        {
            Console.WriteLine("Email template not found.");
            return false;
        }

        string body = await File.ReadAllTextAsync(templatePath, cancellationToken);

        string webUrl = _configuration["AppSettings:WebUrl"]!;

        body = body
            .Replace("{{Title}}", EmailTemplateConsts.OwnerWelcomeTitle)
            .Replace("{{Greeting}}", $"Hello {fullName}")
            .Replace("{{MainText}}", EmailTemplateConsts.OwnerWelcomeMainText)
            .Replace(
                "{{ActionButton}}",
                BuildPrimaryButton(
                    webUrl,
                    EmailTemplateConsts.OwnerWelcomeButtonText))
            .Replace("{{SecondaryText}}", EmailTemplateConsts.OwnerWelcomeSecondaryText)
            .Replace("{{CurrentYear}}", DateTime.UtcNow.Year.ToString());

        string subject = string.Format(
            EmailTemplateConsts.OwnerWelcomeSubject,
            _settings.Email.ApplicationName);

        return await SendEmailAsync(
            email,
            subject,
            body,
            cancellationToken);
    }

    private SmtpClient CreateSmtpClient()
    {
        return new SmtpClient(
            _settings.Smtp.Host,
            _settings.Smtp.Port)
        {
            Credentials = new NetworkCredential(
                _settings.Smtp.UserName,
                _settings.Smtp.Password),
            EnableSsl = true
        };
    }

    private MailMessage CreateMailMessage(
        string to,
        string subject,
        string body)
    {
        MailMessage message = new MailMessage
        {
            From = new MailAddress(
                _settings.Email.From,
                _settings.Email.DisplayName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(to);
        return message;
    }

    private static string? ResolveTemplatePath(string fileName)
    {
        string baseDir = AppContext.BaseDirectory;
        string currentDir = Directory.GetCurrentDirectory();

        string[] possiblePaths =
        [
            Path.Combine(baseDir, "Email", "Templates", fileName),
            Path.Combine(currentDir, "Email", "Templates", fileName),
            Path.Combine(
                currentDir,
                "..",
                "Medicares.Infrastructure",
                "Email",
                "Templates",
                fileName)
        ];

        foreach (string path in possiblePaths)
        {
            if (File.Exists(path))
            {
                return path;
            }
        }

        return null;
    }

    private static string BuildPrimaryButton(
        string url,
        string text)
    {
        return $"""
        <a href="{url}"
           style="
             background: linear-gradient(135deg, #A8EEFF 0%, #19B6E6 100%);
             color: #ffffff;
             text-decoration: none;
             padding: 16px 48px;
             border-radius: 10px;
             font-weight: 600;
             font-size: 16px;
             display: inline-block;">
           {text}
        </a>
        """;
    }
}
