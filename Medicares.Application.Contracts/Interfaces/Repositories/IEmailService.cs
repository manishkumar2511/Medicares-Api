namespace Medicares.Application.Contracts.Interfaces.Repositories;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body, CancellationToken ct = default);
    Task<bool> SendWelcomeEmailAsync(string email, string fullName, CancellationToken ct = default);
}