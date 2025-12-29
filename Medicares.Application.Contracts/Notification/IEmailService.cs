namespace Medicares.Application.Contracts.Notification;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body, CancellationToken ct = default);
    Task<bool> SendWelcomeEmailAsync(string email, string fullName, CancellationToken ct = default);
    Task<bool> SendMfaCodeAsync(string email, string fullName, string code, CancellationToken ct = default);
}