using Medicares.Application.Contracts.Notification;
using Medicares.Domain.Shared.Constant;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using MailSettings = Medicares.Application.Contracts.Models.Mail.MailSettings;

namespace Medicares.Infrastructure.Notification
{
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
            if (string.IsNullOrWhiteSpace(_settings.Email.From))
            {
                throw new InvalidOperationException("Email:From is not configured");
            }

            try
            {
                SendGridClient client = CreateSendGridClient();
                SendGridMessage message = CreateSendGridMessage(to, subject, body);

                Response response = await client.SendEmailAsync(message, cancellationToken);
                return response.StatusCode == System.Net.HttpStatusCode.Accepted
                    || response.StatusCode == System.Net.HttpStatusCode.OK;
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

        public async Task<bool> SendMfaCodeAsync(
                string email,
                string fullName,
                string code,
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

            string codeHtml = $@"
                <div style=""background:#e0f2fe;color:#0284c7;font-size:32px;font-weight:700;letter-spacing:6px;padding:24px 48px;border-radius:12px;display:inline-block;"">
                    {code}
                </div>";

            body = body
                .Replace("{{Title}}", EmailTemplateConsts.MfaCodeTitle)
                .Replace("{{Greeting}}", $"Dear {fullName}")
                .Replace("{{MainText}}", EmailTemplateConsts.MfaCodeMainText)
                .Replace("{{ActionButton}}", codeHtml)
                .Replace("{{SecondaryText}}", EmailTemplateConsts.MfaCodeSecondaryText)
                .Replace("{{CurrentYear}}", DateTime.UtcNow.Year.ToString());

            string subject = string.Format(EmailTemplateConsts.MfaCodeSubject, code);

            return await SendEmailAsync(
               email,
               subject,
               body,
               cancellationToken);
        }

        private SendGridClient CreateSendGridClient()
        {
            string? apiKey = _configuration["SendGrid:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("SendGrid ApiKey missing");

            return new SendGridClient(apiKey);
        }

        private SendGridMessage CreateSendGridMessage(
            string to,
            string subject,
            string body)
        {
            EmailAddress fromAddress = new EmailAddress(
                _settings.Email.From,
                _settings.Email.DisplayName);

            EmailAddress toAddress = new EmailAddress(to);

            SendGridMessage message = new SendGridMessage();
            message.SetFrom(fromAddress);
            message.AddTo(toAddress);
            message.SetSubject(subject);
            message.AddContent(MimeType.Html, body);

            return message;
        }

        private static string? ResolveTemplatePath(string fileName)
        {
            string baseDir = AppContext.BaseDirectory;
            string currentDir = Directory.GetCurrentDirectory();

            string[] possiblePaths = new string[]
            {
                Path.Combine(baseDir, "Email", "Templates", fileName),
                Path.Combine(currentDir, "Email", "Templates", fileName),
                Path.Combine(
                    currentDir,
                    "..",
                    "Medicares.Infrastructure",
                    "Email",
                    "Templates",
                    fileName)
            };

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
}
